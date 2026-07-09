using CloudGameCatalog.Domain.Interfaces;
using MassTransit;

namespace CloudGameCatalog.Consumer.Consumers.PaymentApi.PaymentProcessed;

internal class PaymentProcessedConsumer(IUserGameReadOnlyRepository userGameReadOnlyRepository,
   IUserGameWriteOnlyRepository userGameWriteOnlyRepository,
   IUnitOfWork unitOfWork) : IConsumer<PaymentProcessedEvent>
{
    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        var userGame = await userGameReadOnlyRepository.GetByUserIdAndGameIdAsync(context.Message.UserId, context.Message.GameId);

        if (userGame is null || userGame.Status != Domain.Commom.Enum.UserGameStatus.WaitingPayment) return;

        PaymentProcessedEvent @event = context.Message;

        userGame.SetStatus(context.Message.Status);

        await userGameWriteOnlyRepository.UpdateAsync(userGame);
        await unitOfWork.SaveChangesAsync();
    }
}
