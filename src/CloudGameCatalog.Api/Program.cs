using CloudGameCatalog.Application.Handlers.GameHandler.Create;
using CloudGameCatalog.Application.Handlers.GameHandler.GetById;
using CloudGameCatalog.Application.Handlers.GameHandler.Update;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

GetGameByIdResponse[] sampleTodos =
[
    new GetGameByIdResponse(1,"teste","teste","",0,"teste",DateTime.Now,true),
];

var gamesApi = app.MapGroup("/games");
gamesApi.MapGet("/", () => sampleTodos)
        .WithName("FindGames");

gamesApi.MapGet("/{id}", Results<Ok<GetGameByIdResponse>, NotFound> (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? TypedResults.Ok(todo)
        : TypedResults.NotFound())
    .WithName("GetGameById");

gamesApi.MapPost("/", Results<Ok<CreateGameCommandResponse>, BadRequest> (CreateGameCommand command) =>
    command is not null
        ? TypedResults.Ok(new CreateGameCommandResponse(0, "teste", true))
        : TypedResults.BadRequest())
    .WithName("CreateGame");

gamesApi.MapPut("/{id}", Results<Ok<UpdateGameCommandResponse>, BadRequest> (int id, UpdateGameCommand command) =>
    command is not null
        ? TypedResults.Ok(new UpdateGameCommandResponse(0, "teste", true))
        : TypedResults.BadRequest())
    .WithName("UpdateGame");

app.Run();

[JsonSerializable(typeof(GetGameByIdResponse[]))]
[JsonSerializable(typeof(CreateGameCommand))]
[JsonSerializable(typeof(UpdateGameCommand))]
[JsonSerializable(typeof(CreateGameCommandResponse))]
[JsonSerializable(typeof(UpdateGameCommandResponse))]
[JsonSerializable(typeof(CreateGameCommandResponse[]))]
[JsonSerializable(typeof(UpdateGameCommandResponse[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}
