using CloudGameCatalog.Domain.Commom;

namespace CloudGameCatalog.Domain.Parameters;

public class FindGamesParameter : PaginationParameters
{
    public string? Name { get; set; }

    public bool? Active { get; set; }
}
