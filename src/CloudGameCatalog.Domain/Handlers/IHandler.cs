using CloudGameCatalog.Domain.Commom;

namespace CloudGameCatalog.Domain.Handlers;

public interface IHandler<TCommand, TResponse>
    where TCommand : ICommand, new()
    where TResponse : IResponse
{
    Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken);
}

public interface IHandler<TCommand>
    where TCommand : ICommand, new()
{
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
