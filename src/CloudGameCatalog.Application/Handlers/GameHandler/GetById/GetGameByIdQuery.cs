using CloudGameCatalog.Domain.Handlers;

namespace CloudGameCatalog.Application.Handlers.GameHandler.GetById;

public sealed class GetGameByIdQuery : ICommand
{
    public int Id { get; set; }
}
