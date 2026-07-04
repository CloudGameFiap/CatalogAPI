using System.Text.Json.Serialization;

namespace CloudGameCatalog.Domain.Commom.Events
{
    [method: JsonConstructor]
    public class OrderPlacedEvent(int userId, int gameId, decimal price)
    {
        public int UserId { get; private set; } = userId;

        public int GameId { get; private set; } = gameId;

        public decimal Price { get; private set; } = price;
    }
}
