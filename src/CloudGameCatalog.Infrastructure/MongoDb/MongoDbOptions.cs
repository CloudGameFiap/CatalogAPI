namespace CloudGameCatalog.Infrastructure.MongoDb;

public class MongoDbOptions
{
    public const string SectionName = "MongoDbCache";
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string CollectionName { get; set; } = string.Empty;
}