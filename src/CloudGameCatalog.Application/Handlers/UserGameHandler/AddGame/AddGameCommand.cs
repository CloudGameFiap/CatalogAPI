using CloudGameCatalog.Domain.Handlers;
using System.Text.Json.Serialization;

namespace CloudGameCatalog.Application.Handlers.UserGameHandler.AddGame
{
    public sealed class AddGameCommand : ICommand
    {
        public int GameId { get; set; }

        public decimal Price { get; set; }  

        [JsonIgnore]
        public int UserId { get; set; }
    }   
}
