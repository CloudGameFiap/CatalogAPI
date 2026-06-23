using CloudGameCatalog.Application.Handlers.GameHandler.Create;
using CloudGameCatalog.Application.Handlers.GameHandler.Find;
using CloudGameCatalog.Application.Handlers.GameHandler.GetById;
using CloudGameCatalog.Application.Handlers.GameHandler.Update;
using CloudGameCatalog.Application.Settings;
using CloudGameCatalog.Domain.Commom;
using CloudGameCatalog.Domain.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var gamesApi = app.MapGroup("/games");

gamesApi.MapGet("/", () => FindGamesAsync)
        .WithName("FindGames");

gamesApi.MapGet("/{id}", GetGameByIdAsync)
    .WithName("GetGameById");

gamesApi.MapPost("/", CreateGameAsync)
    .WithName("CreateGame");

gamesApi.MapPut("/", UpdateGameAsync)
    .WithName("UpdateGame");

static async Task<Results<Ok<Result<Pagination<FindGamesQueryResponse>>>, NotFound>> FindGamesAsync(FindGamesQuery request, IHandler<FindGamesQuery, Pagination<FindGamesQueryResponse>> handler,
    CancellationToken ct)
{
    var result = await handler.HandleAsync(request, ct);

    return result.IsSuccess ? TypedResults.Ok(result)
        : TypedResults.NotFound();
}

static async Task<Results<Ok<Result<GetGameByIdQueryResponse>>, NotFound<Result<GetGameByIdQueryResponse>>>> GetGameByIdAsync(int id, IHandler<GetGameByIdQuery, GetGameByIdQueryResponse> handler,
    CancellationToken ct)
{
    var result = await handler.HandleAsync(new GetGameByIdQuery() { Id = id }, ct);

    return result.IsSuccess ? TypedResults.Ok(result)
        : TypedResults.NotFound(result);
}

static async Task<Results<Ok<Result<CreateGameCommandResponse>>, BadRequest<Result<CreateGameCommandResponse>>>> CreateGameAsync(CreateGameCommand command, IHandler<CreateGameCommand, CreateGameCommandResponse> handler,
    CancellationToken ct)
{
    var result = await handler.HandleAsync(command, ct);

    return result.IsSuccess ? TypedResults.Ok(result)
        : TypedResults.BadRequest(result);
}

static async Task<Results<Ok<Result<UpdateGameCommandResponse>>, BadRequest<Result<UpdateGameCommandResponse>>>> UpdateGameAsync(UpdateGameCommand command, IHandler<UpdateGameCommand, UpdateGameCommandResponse> handler,
    CancellationToken ct)
{    
    var result = await handler.HandleAsync(command, ct);

    return result.IsSuccess ? TypedResults.Ok(result)
        : TypedResults.BadRequest(result);
}

app.Run();

[JsonSerializable(typeof(GetGameByIdQueryResponse[]))]
[JsonSerializable(typeof(CreateGameCommand))]
[JsonSerializable(typeof(UpdateGameCommand))]
[JsonSerializable(typeof(CreateGameCommandResponse))]
[JsonSerializable(typeof(UpdateGameCommandResponse))]
[JsonSerializable(typeof(CreateGameCommandResponse[]))]
[JsonSerializable(typeof(UpdateGameCommandResponse[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
