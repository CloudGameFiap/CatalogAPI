using CloudGameCatalog.Application.Interfaces;
using CloudGameCatalog.Application.Models.Search;
using CloudGameCatalog.Domain.Commom.Events;
using CloudGameCatalog.Domain.Interfaces;
using MassTransit;

namespace CloudGameCatalog.Consumer.Consumers.CatalogApi.GameIndexRequested;

internal class GameIndexRequestedConsumer(
    IGameReadOnlyRepository gameReadOnlyRepository,
    IGameSearchService gameSearchService,
    ILogger<GameIndexRequestedConsumer> logger)
    : IConsumer<GameIndexRequestedEvent>
{
    public async Task Consume(ConsumeContext<GameIndexRequestedEvent> context)
    {
        logger.LogInformation("Receive event GameIndexRequestedEvent.");

        if (context.Message is null) return;

        var gameId = context.Message.GameId;

        // Momento da leitura = versão externa. Uma leitura posterior sempre vê dado igual ou mais novo,
        // então uma reindexação atrasada nunca sobrescreve uma mais recente.
        var version = DateTime.UtcNow.Ticks;

        var game = await gameReadOnlyRepository.GetByIdAsync(gameId);

        // Se lançar exceção (Elasticsearch fora do ar), o MassTransit faz retry e depois manda para a fila de erro.
        if (game is null)
            await gameSearchService.DeleteAsync(gameId, context.CancellationToken);
        else
            await gameSearchService.IndexAsync(GameSearchDocument.From(game), version, context.CancellationToken);

        logger.LogInformation("Received event GameIndexRequestedEvent processed.");
    }
}
