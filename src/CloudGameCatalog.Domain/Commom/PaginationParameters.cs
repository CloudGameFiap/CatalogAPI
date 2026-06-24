namespace CloudGameCatalog.Domain.Commom;

public class PaginationParameters
{
    public int? PageNumber { get; set; } = 1;

    public int? PageSize { get; set; } = 10;

    public int Skip => (PageNumber.GetValueOrDefault() - 1) * PageSize.GetValueOrDefault();
}
