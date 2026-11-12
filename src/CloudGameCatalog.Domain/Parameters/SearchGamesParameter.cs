using CloudGameCatalog.Domain.Commom;

namespace CloudGameCatalog.Domain.Parameters;

public class SearchGamesParameter : PaginationParameters
{
    public string? Text { get; set; }

    public string? Genre { get; set; }

    public decimal? MinPrice { get; set; }

    public decimal? MaxPrice { get; set; }
}
