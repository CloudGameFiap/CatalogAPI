using CloudGameCatalog.Application.Extensions;
using CloudGameCatalog.Consumer.Consumers.UserApi.UserCreated;
using CloudGameCatalog.Infrastructure.EntityFramework;
using CloudGameCatalog.Infrastructure.Extensions;
using MassTransit;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplicationHandlers()
    .AddInfrastructureServices(builder.Configuration);

builder.Services.AddMassTransit(bus =>
{
    bus.AddConsumer<UserCreatedConsumer>();

    bus.UsingRabbitMq((ctx, cfg) =>
    {
        var rabbitMqSection = builder.Configuration.GetRequiredSection("RabbitMQ")!;
        var host = rabbitMqSection["Host"]!;
        var username = rabbitMqSection["Username"]!;
        var password = rabbitMqSection["Password"]!;

        cfg.Host(host, "/", h =>
        {
            h.Username(username);
            h.Password(password);
        });

        cfg.ReceiveEndpoint("CloudGame.Domain.Events.User:UserCreatedEvent", e =>
        {
            e.Consumer<UserCreatedConsumer>(ctx);
        });
    });
});

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
await using (var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
{
    await appDbContext.Database.EnsureCreatedAsync();
}

await app.RunAsync();
