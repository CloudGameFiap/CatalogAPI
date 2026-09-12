using CloudGame.Domain.Events.User;
using CloudGameCatalog.Domain.Entities;
using CloudGameCatalog.Domain.Interfaces;
using MassTransit;

namespace CloudGameCatalog.Consumer.Consumers.UserApi.UserCreated;

internal class UserCreatedConsumer(
    IUserWriteOnlyRepository userWriteOnlyRepository,
    IUserReadOnlyRepository userReadOnlyRepository,
    IUnitOfWork unitOfWork,
    ILogger<UserCreatedConsumer> logger)
    : IConsumer<UserCreatedEvent>
{
    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        logger.LogInformation("Receive event UserCreatedEvent.");

        if (context.Message is null) return;

        var existingUser = await userReadOnlyRepository.GetByIdAsync(context.Message.Id);

        if (existingUser is not null) return;

        UserCreatedEvent @event = context.Message;

        User user = User.Create(
            Id: @event.Id,
            Name: @event.Name,
            Email: @event.Email,
            BirthDate: @event.BirthDate,
            Active: @event.Active,
            CreatedAt: @event.CreatedAt,
            IsAdmin: @event.IsAdmin
        );

        await userWriteOnlyRepository.AddAsync(user);

        await unitOfWork.SaveChangesAsync();

        logger.LogInformation("Received event UserCreatedEvent processed.");
    }
}
