using CloudGameCatalog.Application.Interfaces;
using CloudGameCatalog.Application.Models.Search;
using CloudGameCatalog.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CloudGameCatalog.Infrastructure.BackgroundServices;

/// <summary>
/// Garante que o índice exista na subida da aplicação. Quando o índice é criado agora,
/// faz a carga inicial a partir do SQL Server (fonte da verdade).
/// </summary>
public sealed class SearchIndexInitializer(
    IGameSearchService search,
    IServiceScopeFactory scopeFactory,
    ILogger<SearchIndexInitializer> logger) : BackgroundService
{
    private const int MaxAttempts = 10;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        for (var attempt = 1; attempt <= MaxAttempts && !stoppingToken.IsCancellationRequested; attempt++)
        {
            try
            {
                var created = await search.EnsureIndexAsync(stoppingToken);

                if (created)
                    await ReindexAllAsync(stoppingToken);

                return;
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning(ex, "Elasticsearch indisponível (tentativa {Attempt}/{MaxAttempts})", attempt, MaxAttempts);

                if (attempt < MaxAttempts)
                    await Task.Delay(TimeSpan.FromSeconds(5 * attempt), stoppingToken);
            }
        }

        logger.LogError("Não foi possível garantir o índice de busca na inicialização");
    }

    private async Task ReindexAllAsync(CancellationToken ct)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var gameReadOnlyRepository = scope.ServiceProvider.GetRequiredService<IGameReadOnlyRepository>();

        var games = await gameReadOnlyRepository.GetAllAsync();
        var indexed = await search.BulkIndexAsync(games.Select(GameSearchDocument.From).ToList(), ct);

        logger.LogInformation("Carga inicial do índice concluída: {Indexed}/{Total} jogos", indexed, games.Count);
    }
}
