using CloudGameCatalog.Domain.Commom.Enum;

namespace CloudGameCatalog.Consumer.Consumers.PaymentApi.PaymentProcessed
{
    public class PaymentProcessedEvent
    {
        public int GameId { get; set; }

        public int UserId { get; set; }

        public UserGameStatus Status { get; set; }
    }
}
