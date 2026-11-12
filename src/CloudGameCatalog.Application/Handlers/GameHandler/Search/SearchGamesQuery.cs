using CloudGameCatalog.Domain.Handlers;
using CloudGameCatalog.Domain.Parameters;

namespace CloudGameCatalog.Application.Handlers.GameHandler.Search;

public sealed class SearchGamesQuery : ICommand
{
    public SearchGamesQuery()
    {
        Parameters = new SearchGamesParameter();
    }

    public SearchGamesQuery(SearchGamesParameter parameters)
    {
        Parameters = parameters;
    }
    public SearchGamesParameter Parameters { get; set; }
}
