namespace CloudGameCatalog.Infrastructure.Search;

public class ElasticsearchOptions
{
    public const string SectionName = "Elasticsearch";
    public string Url { get; set; } = "http://localhost:9200";
    public string IndexName { get; set; } = "games";
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ApiKey { get; set; } // Elastic Cloud / clusters com API key (tem precedência sobre usuário/senha)
    public int NumberOfReplicas { get; set; } = 0; // 0 em dev (single node); 1+ no serviço gerenciado
}
