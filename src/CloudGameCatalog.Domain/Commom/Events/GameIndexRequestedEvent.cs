using System.Text.Json.Serialization;

namespace CloudGameCatalog.Domain.Commom.Events
{
    [method: JsonConstructor]
    public class GameIndexRequestedEvent(int gameId)
    {
        public int GameId { get; private set; } = gameId;
    }
}
