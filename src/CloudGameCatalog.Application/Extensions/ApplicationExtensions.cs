using CloudGameCatalog.Application.Handlers.GameHandler.Create;
using CloudGameCatalog.Application.Handlers.GameHandler.Find;
using CloudGameCatalog.Application.Handlers.GameHandler.GetById;
using CloudGameCatalog.Application.Handlers.GameHandler.Update;
using CloudGameCatalog.Domain.Commom;
using CloudGameCatalog.Domain.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace CloudGameCatalog.Application.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationHandlers(this IServiceCollection services)
    {
        services.AddScoped<IHandler<CreateGameCommand, CreateGameCommandResponse>, CreateGameCommandHandler>();
        services.AddScoped<IHandler<UpdateGameCommand, UpdateGameCommandResponse>, UpdateGameCommandHandler>();

        services.AddScoped<IHandler<GetGameByIdQuery, GetGameByIdQueryResponse>, GetGameByIdQueryHandler>();
        services.AddScoped<IHandler<FindGamesQuery, Pagination<FindGamesQueryResponse>>, FindGamesQueryHandler>();

        return services;
    }
}
