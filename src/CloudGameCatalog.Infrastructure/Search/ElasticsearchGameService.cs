using CloudGameCatalog.Application.Interfaces;
using CloudGameCatalog.Application.Models.Search;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Analysis;
using Elastic.Clients.Elasticsearch.Core.Search;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CloudGameCatalog.Infrastructure.Search;

public sealed class ElasticsearchGameService : IGameSearchService
{
    private const string TextAnalyzer = "game_text";

    private readonly ElasticsearchClient _client;
    private readonly ElasticsearchOptions _options;
    private readonly ILogger<ElasticsearchGameService> _logger;

    public ElasticsearchGameService(
        ElasticsearchClient client,
        IOptions<ElasticsearchOptions> options,
        ILogger<ElasticsearchGameService> logger)
    {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }

    private string Index => _options.IndexName;

    public async Task<bool> EnsureIndexAsync(CancellationToken ct = default)
    {
        var exists = await _client.Indices.ExistsAsync(Index, ct);

        // Sem resposta válida (cluster fora do ar) o Exists vem false; não tenta criar às cegas.
        if (!exists.IsValidResponse && exists.ApiCallDetails?.HttpStatusCode != 404)
            throw new InvalidOperationException($"Falha ao verificar índice '{Index}': {exists.DebugInformation}");

        if (exists.Exists) return false;

        var response = await _client.Indices.CreateAsync<GameSearchDocument>(Index, c => c
            .Settings(s => s
                .NumberOfShards(1)
                .NumberOfReplicas(_options.NumberOfReplicas)
                .Analysis(a => a
                    .Analyzers(an => an
                        // lowercase + asciifolding: "Pokemon" encontra "Pokémon", "acao" encontra "ação"
                        .Custom(TextAnalyzer, ca => ca
                            .Tokenizer("standard")
                            .Filter(["lowercase", "asciifolding"])))))
            .Mappings(m => m
                .Properties(p => p
                    .Keyword(d => d.Id)
                    .Text(d => d.Name, t => t
                        .Analyzer(TextAnalyzer)
                        .Fields(f => f.Keyword("raw"))) // usado no desempate da ordenação
                    .Text(d => d.Description, t => t.Analyzer(TextAnalyzer))
                    .Keyword(d => d.ImageUrl!, k => k.Index(false))
                    .Keyword(d => d.Genre)
                    .Boolean(d => d.Active)
                    .ScaledFloatNumber(d => d.Price, n => n.ScalingFactor(100))
                    .Date(d => d.ReleaseDate)
                    .Date(d => d.IndexedAt))),
            ct);

        if (!response.IsValidResponse)
            throw new InvalidOperationException($"Falha ao criar índice '{Index}': {response.DebugInformation}");

        _logger.LogInformation("Índice {Index} criado", Index);

        return true;
    }

    public async Task IndexAsync(GameSearchDocument document, long version, CancellationToken ct = default)
    {
        // Versionamento externo: se duas atualizações do mesmo jogo chegarem fora de ordem,
        // a mais antiga é rejeitada (409) e não sobrescreve a mais nova.
        var response = await _client.IndexAsync(document, i => i
            .Index(Index)
            .Id(document.Id.ToString())
            .Version(version)
            .VersionType(VersionType.External), ct);

        if (response.IsValidResponse) return;

        if (response.ApiCallDetails?.HttpStatusCode == 409)
        {
            _logger.LogDebug("Versão mais nova do jogo {GameId} já indexada; ignorando", document.Id);
            return;
        }

        throw new InvalidOperationException($"Falha ao indexar jogo {document.Id}: {response.DebugInformation}");
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var response = await _client.DeleteAsync(new DeleteRequest(Index, id.ToString()), ct);

        if (!response.IsValidResponse && response.ApiCallDetails?.HttpStatusCode != 404)
            throw new InvalidOperationException($"Falha ao remover jogo {id} do índice: {response.DebugInformation}");
    }

    public async Task<int> BulkIndexAsync(IReadOnlyCollection<GameSearchDocument> documents, CancellationToken ct = default)
    {
        if (documents.Count == 0) return 0;

        var response = await _client.BulkAsync(b => b
            .Index(Index)
            .IndexMany(documents, (d, doc) => d.Id(doc.Id.ToString())), ct);

        if (!response.IsValidResponse && !response.Errors)
            throw new InvalidOperationException($"Falha no bulk: {response.DebugInformation}");

        var failed = response.ItemsWithErrors.ToList();

        foreach (var item in failed)
            _logger.LogError("Erro no bulk do jogo {Id}: {Reason}", item.Id, item.Error?.Reason);

        return documents.Count - failed.Count;
    }

    public async Task<GameSearchResult> SearchAsync(GameSearchRequest request, CancellationToken ct = default)
    {
        var page = Math.Max(1, request.Page);
        var size = Math.Clamp(request.PageSize, 1, 100);
        var hasQuery = !string.IsNullOrWhiteSpace(request.Query);

        var filters = new List<Query>
        {
            new TermQuery { Field = Infer.Field<GameSearchDocument>(d => d.Active), Value = true }
        };

        if (!string.IsNullOrWhiteSpace(request.Genre))
            filters.Add(new TermQuery { Field = Infer.Field<GameSearchDocument>(d => d.Genre), Value = request.Genre });

        if (request.MinPrice.HasValue || request.MaxPrice.HasValue)
            filters.Add(new NumberRangeQuery(Infer.Field<GameSearchDocument>(d => d.Price))
            {
                Gte = (double?)request.MinPrice,
                Lte = (double?)request.MaxPrice
            });

        var boolQuery = new BoolQuery
        {
            // Fuzzy: fuzziness AUTO tolera 1 erro em termos de 3–5 letras e 2 erros acima disso.
            // PrefixLength(1) exige a 1ª letra correta — reduz ruído e custo.
            Must = [hasQuery
                ? new MultiMatchQuery
                {
                    Query = request.Query!,
                    Fields = Fields.FromFields([Infer.Field<GameSearchDocument>(d => d.Name, 3), Infer.Field<GameSearchDocument>(d => d.Description)]),
                    Type = TextQueryType.BestFields,
                    Fuzziness = new Fuzziness("AUTO"),
                    PrefixLength = 1,
                    MaxExpansions = 50
                }
                : new MatchAllQuery()],
            // Filtros não afetam o score (contexto de filtro, cacheável)
            Filter = filters
        };

        // Bônus de relevância para correspondência exata da frase no título
        if (hasQuery)
            boolQuery.Should = [new MatchPhraseQuery
            {
                Field = Infer.Field<GameSearchDocument>(d => d.Name),
                Query = request.Query!,
                Boost = 5
            }];

        var searchRequest = new SearchRequest<GameSearchDocument>(Index)
        {
            From = (page - 1) * size,
            Size = size,
            TrackTotalHits = new TrackHits(true),
            Query = boolQuery,
            // Ordenação por relevância (_score) com desempate determinístico por título
            Sort =
            [
                new SortOptions { Score = new ScoreSort { Order = SortOrder.Desc } },
                new SortOptions { Field = new FieldSort { Field = Infer.Field<GameSearchDocument>(d => d.Name.Suffix("raw")), Order = SortOrder.Asc } }
            ]
        };

        var response = await _client.SearchAsync<GameSearchDocument>(searchRequest, ct);

        if (!response.IsValidResponse)
            throw new InvalidOperationException($"Falha na busca: {response.DebugInformation}");

        var items = response.Hits
            .Where(h => h.Source is not null)
            .Select(h => new GameSearchHit(h.Source!, h.Score))
            .ToList();

        return new GameSearchResult(response.Total, page, size, items);
    }
}
