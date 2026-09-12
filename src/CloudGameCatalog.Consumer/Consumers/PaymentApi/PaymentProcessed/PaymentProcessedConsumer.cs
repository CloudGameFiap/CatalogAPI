using CloudGameCatalog.Consumer.Consumers.UserApi.UserCreated;
using CloudGameCatalog.Domain.Interfaces;
using MassTransit;

namespace CloudGameCatalog.Consumer.Consumers.PaymentApi.PaymentProcessed;

internal class PaymentProcessedConsumer(IUserGameReadOnlyRepository userGameReadOnlyRepository,
   IUserGameWriteOnlyRepository userGameWriteOnlyRepository,
   IUnitOfWork unitOfWork,
   ILogger<UserCreatedConsumer> logger) : IConsumer<PaymentProcessedEvent>
{
    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        logger.LogInformation("PaymentProcessedEvent received.");

        var userGame = await userGameReadOnlyRepository.GetByUserIdAndGameIdAsync(context.Message.UserId, context.Message.GameId);

        if (userGame is null || userGame.Status != Domain.Commom.Enum.UserGameStatus.WaitingPayment) return;

        PaymentProcessedEvent @event = context.Message;

        userGame.SetStatus(context.Message.Status);

        await userGameWriteOnlyRepository.UpdateAsync(userGame);
        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("PaymentProcessedEvent processed.");
    }
}
