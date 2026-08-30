namespace CloudGameCatalog.Infrastructure.MongoDb;

public class MongoDbOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "CloudGameCache";
    public string CollectionName { get; set; } = "CloudGame";
}