using CloudGameCatalog.Application.Extensions;
using CloudGameCatalog.Consumer.Consumers.UserApi.UserCreated;
using CloudGameCatalog.Infrastructure.EntityFramework;
using CloudGameCatalog.Infrastructure.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting up the application...");

    var builder = Host.CreateApplicationBuilder(args);

    builder.Services.AddSerilog((hostingContext, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(builder.Configuration);
    });

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

    Log.Information("The application has been built, and star the pipeline setup has started.");

    Log.Information("Pipeline successfully configured and application initialized...");

    await app.RunAsync();
}
catch (Exception ex) when (ex.GetType().Name != "HostAbortedException")
{
    Log.Fatal(ex, "Application failed to start");
}
catch (Exception)
{
    throw;
}
finally
{
    Log.Information("Shutting down the application...");
    Log.CloseAndFlush();
}