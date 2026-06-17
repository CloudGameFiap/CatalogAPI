using CloudGameCatalog.Domain.Handlers;
using CloudGameCatalog.Domain.Parameters;

namespace CloudGameCatalog.Application.Handlers.GameHandler.Find;

public sealed class FindGamesQuery : ICommand
{
    public FindGamesQuery()
    {
        Parameters = new FindGamesParameter();
    }

    public FindGamesQuery(FindGamesParameter parameters)
    {
        Parameters = parameters;
    }
    public FindGamesParameter Parameters { get; set; }
}
