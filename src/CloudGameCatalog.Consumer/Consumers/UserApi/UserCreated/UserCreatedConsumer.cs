using CloudGameCatalog.Domain.Interfaces;
using MassTransit;

namespace CloudGameCatalog.Consumer.Consumers.UserApi.UserCreated;

internal class UserCreatedConsumer(
    ILogger<UserCreatedConsumer> logger,
    IUserWriteOnlyRepository userWriteOnlyRepository,
    IUserReadOnlyRepository userReadOnlyRepository,
    IUnitOfWork unitOfWork)
    : IConsumer<UserCreatedEvent>
{
    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {


    }
}
