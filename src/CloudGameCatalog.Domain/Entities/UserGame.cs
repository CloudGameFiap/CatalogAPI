using CloudGameCatalog.Domain.Commom.Enum;

namespace CloudGameCatalog.Domain.Entities;

public class UserGame: Entity<int>
{    
    public int UserId { get; private set; }

    public User User { get; set; }

    public int GameId { get; private set; }

    public Game Game { get; set; }

    public UserGameStatus Status { get; private set; }

    public decimal Price { get; private set; }

    public UserGame(int userId, int gameId, decimal price)
    {
        UserId = userId;
        GameId = gameId;
        Price = price;
        Status = UserGameStatus.WaitingPayment;
    }

    public void SetStatus(UserGameStatus status)
    {
        Status = status;
    }    

    private UserGame() { }
}
