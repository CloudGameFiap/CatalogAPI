using CloudGameCatalog.Api.Extensions;
using CloudGameCatalog.Application.Extensions;
using CloudGameCatalog.Application.Handlers.GameHandler.Create;
using CloudGameCatalog.Application.Handlers.GameHandler.Find;
using CloudGameCatalog.Application.Handlers.GameHandler.GetById;
using CloudGameCatalog.Application.Handlers.GameHandler.Update;
using CloudGameCatalog.Application.Handlers.UserGameHandler.AddGame;
using CloudGameCatalog.Application.Settings;
using CloudGameCatalog.Domain.Commom;
using CloudGameCatalog.Domain.Handlers;
using CloudGameCatalog.Domain.Parameters;
using CloudGameCatalog.Infrastructure.EntityFramework;
using CloudGameCatalog.Infrastructure.Extensions;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting up the application...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((hostingContext, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(hostingContext.Configuration);
    });

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    //builder.Services.AddOpenApi();

    builder.Services.AddApplicationHandlers()
        .AddInfrastructureServices(builder.Configuration);

    var jwtSettingsSection = builder.Configuration.GetRequiredSection("JwtSettings");
    builder.Services.Configure<JwtSettings>(jwtSettingsSection);

    var encriptKey = jwtSettingsSection.GetValue<string>("EncriptKey")!;
    var key = Encoding.ASCII.GetBytes(encriptKey);
    builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

    builder.Services.AddAuthorization();

    builder.Services.AddMassTransit(bus =>
    {
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

            cfg.ConfigureEndpoints(ctx);

            //cfg.Publish<UserCreatedEvent>(p =>
            //{
            //    p.ExchangeType = RabbitMQ.Client.ExchangeType.Direct;
            //});
        });
    });

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHealthChecks("/health");

    Log.Information("The application has been built, and star the pipeline setup has started.");

    await using (var scope = app.Services.CreateAsyncScope())
    await using (var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
    {
        await appDbContext.Database.MigrateAsync();
    }

    var gamesApi = app.MapGroup("/api/games").RequireAuthorization();

    gamesApi.MapGet("/", FindGamesAsync)
            .WithName("FindGames").AllowAnonymous();

    gamesApi.MapGet("/{id:int}", GetGameByIdAsync)
        .WithName("GetGameById");

    gamesApi.MapPost("/", CreateGameAsync)
        .WithName("CreateGame");

    gamesApi.MapPut("/", UpdateGameAsync)
        .WithName("UpdateGame");

    var userGamesApi = app.MapGroup("/api/user-games").RequireAuthorization();

    //userGamesApi.MapGet("/{id:int}", GetGamesByUserIdAsync)
    //    .WithName("GetGamesByUserIdAsync");

    userGamesApi.MapPost("/", AddGameAsync)
        .WithName("AddGameAsync");

    static async Task<Results<Ok<Result<Pagination<FindGamesQueryResponse>>>, NotFound>> FindGamesAsync([AsParameters] FindGamesParameter parameters, [FromServices] IHandler<FindGamesQuery, Pagination<FindGamesQueryResponse>> handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new FindGamesQuery(parameters), ct);

        return result.IsSuccess ? TypedResults.Ok(result)
            : TypedResults.NotFound();
    }

    static async Task<Results<Ok<Result<GetGameByIdQueryResponse>>, NotFound<Result<GetGameByIdQueryResponse>>>> GetGameByIdAsync([FromRoute] int id, [FromServices] IHandler<GetGameByIdQuery, GetGameByIdQueryResponse> handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(new GetGameByIdQuery() { Id = id }, ct);

        return result.IsSuccess ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }

    static async Task<Results<Ok<Result<CreateGameCommandResponse>>, BadRequest<Result<CreateGameCommandResponse>>>> CreateGameAsync([FromBody] CreateGameCommand command, [FromServices] IHandler<CreateGameCommand, CreateGameCommandResponse> handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(command, ct);

        return result.IsSuccess ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }

    static async Task<Results<Ok<Result<UpdateGameCommandResponse>>, BadRequest<Result<UpdateGameCommandResponse>>>> UpdateGameAsync([FromBody] UpdateGameCommand command, [FromServices] IHandler<UpdateGameCommand, UpdateGameCommandResponse> handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(command, ct);

        return result.IsSuccess ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }

    static async Task<Results<Ok<Result<AddGameCommandResponse>>, BadRequest<Result<AddGameCommandResponse>>>> AddGameAsync([FromBody] AddGameCommand command, [FromServices] IHandler<AddGameCommand, AddGameCommandResponse> handler,
    HttpContext httpContext,  CancellationToken ct)
    {
        var userId = int.Parse(httpContext.User.Claims.FirstOrDefault(s => s.Type == "UserId")?.Value ?? "0");

        command.UserId = userId;

        var result = await handler.HandleAsync(command, ct);

        return result.IsSuccess ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options =>
        {
            options.AddDocument("v1", "v1", "/openapi/v1.json")
                .WithTitle("CloudGameCatalog")
                .WithTheme(ScalarTheme.Mars)
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });
    }

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