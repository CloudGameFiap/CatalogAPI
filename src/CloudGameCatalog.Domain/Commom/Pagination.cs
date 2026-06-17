using CloudGameCatalog.Domain.Handlers;

namespace CloudGameCatalog.Domain.Commom;

public class Pagination<TResult>(IReadOnlyCollection<TResult> items, int count) : IResponse where TResult : class
{
    public IReadOnlyCollection<TResult> Items { get; } = items;

    public int Count { get; } = count;
}
