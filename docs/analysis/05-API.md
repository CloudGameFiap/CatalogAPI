# Análise da Camada de API

**Projeto:** `CloudGameCatalog.Api`
**Tipo:** ASP.NET Core Minimal API (.NET 10.0)

---

## Tecnologias Utilizadas

| Tecnologia | Uso |
|-----------|-----|
| `WebApplication.CreateSlimBuilder` | Builder leve (sem MVC) |
| Minimal APIs | Endpoints definidos em `Program.cs` |
| JWT Bearer Authentication | Autenticação por token |
| OpenAPI / Swagger | Documentação (apenas em Development) |
| Source Generators JSON | Serialização AOT-friendly |

---

## Endpoints Registrados

Base path: `/games`

| Método | Rota | Nome | Retorno Esperado |
|--------|------|------|-----------------|
| `GET` | `/games/` | `FindGames` | `GetGameByIdResponse[]` |
| `GET` | `/games/{id}` | `GetGameById` | `Ok<GetGameByIdResponse>` ou `NotFound` |
| `POST` | `/games/` | `CreateGame` | `Ok<CreateGameCommandResponse>` ou `BadRequest` |
| `PUT` | `/games/{id}` | `UpdateGame` | `Ok<UpdateGameCommandResponse>` ou `BadRequest` |

---

## Estado Atual dos Endpoints

**Todos os endpoints retornam dados hardcoded.** Os handlers não estão conectados.

```csharp
// GET /games/ — retorna array fixo
GetGameByIdResponse[] sampleTodos =
[
    new GetGameByIdResponse(1, "teste", "teste", "", 0, "teste", DateTime.Now, true),
];
gamesApi.MapGet("/", () => sampleTodos);

// GET /games/{id} — filtra do array fixo
gamesApi.MapGet("/{id}", Results<Ok<GetGameByIdResponse>, NotFound> (int id) =>
    sampleTodos.FirstOrDefault(a => a.Id == id) is { } todo
        ? TypedResults.Ok(todo)
        : TypedResults.NotFound());

// POST /games/ — ignora o command, retorna resposta fixa
gamesApi.MapPost("/", Results<Ok<CreateGameCommandResponse>, BadRequest> (CreateGameCommand command) =>
    command is not null
        ? TypedResults.Ok(new CreateGameCommandResponse(0, "teste", true))
        : TypedResults.BadRequest());

// PUT /games/{id} — ignora o command, retorna resposta fixa
gamesApi.MapPut("/{id}", Results<Ok<UpdateGameCommandResponse>, BadRequest> (int id, UpdateGameCommand command) =>
    command is not null
        ? TypedResults.Ok(new UpdateGameCommandResponse(0, "teste", true))
        : TypedResults.BadRequest());
```

---

## Autenticação JWT

```csharp
// Configuração no Program.cs
var encriptKey = jwtSettingsSection.GetValue<string>("EncrictKey")!;
var key = Encoding.ASCII.GetBytes(encriptKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer   = false,
            ValidateAudience = false
        };
    });
```

**Problemas identificados:**

| Problema | Detalhe |
|---------|---------|
| `RequireHttpsMetadata = false` | Aceita tokens via HTTP — inseguro em produção |
| `ValidateIssuer = false` | Não valida o emissor do token |
| `ValidateAudience = false` | Não valida o público-alvo do token |
| Nenhum endpoint usa `[Authorize]` | Autenticação configurada mas não aplicada |
| Endpoint de geração de token ausente | Não há rota `/auth/login` ou similar |
| `EncriptKey` apenas no `appsettings.Development.json` | Em produção a chave não está configurada — vai lançar exceção |

---

## Serialização JSON — Source Generators

```csharp
[JsonSerializable(typeof(GetGameByIdResponse[]))]
[JsonSerializable(typeof(CreateGameCommand))]
[JsonSerializable(typeof(UpdateGameCommand))]
[JsonSerializable(typeof(CreateGameCommandResponse))]
[JsonSerializable(typeof(UpdateGameCommandResponse))]
[JsonSerializable(typeof(CreateGameCommandResponse[]))]
[JsonSerializable(typeof(UpdateGameCommandResponse[]))]
internal partial class AppJsonSerializerContext : JsonSerializerContext { }
```

Uso de `JsonSerializerContext` permite:
- Serialização AOT (Ahead-of-Time) compatível com `PublishSingleFile` e NativeAOT
- Performance de serialização melhor
- Eliminação de reflection em runtime

**Lacuna:** `FindGamesQueryResponse` e `Pagination<T>` **não estão registrados** no `AppJsonSerializerContext`.

---

## Configuração de Ambientes

### appsettings.json (Produção)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "Default": "Server=sqlserver;..."
  },
  "AllowedHosts": "*"
}
```

### appsettings.Development.json

```json
{
  "JwtSettings": {
    "EncrictKey": "...",
    "Audience": "CloudGameFiap",
    "Issuer": "CloudGameFiap",
    "ExpiresInMinutes": 60
  },
  "ConnectionStrings": {
    "Default": "Server=sqlserver;..."
  }
}
```

> `JwtSettings` ausente no `appsettings.json` de produção — `GetRequiredSection("JwtSettings")` vai lançar exceção ao iniciar em produção.

---

## OpenAPI

```csharp
builder.Services.AddOpenApi();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
```

- Habilitado apenas em Development
- Nenhum endpoint tem `.WithSummary()`, `.WithDescription()` ou `.Produces<T>()` — documentação automática mínima

---

## launchSettings.json

| Perfil | URL | Descrição |
|--------|-----|-----------|
| `http` | `http://localhost:5201/games` | Desenvolvimento local |
| `Docker` | `http://localhost:{port}/games` | Porta dinâmica via Docker |

---

## Avaliação da Camada de API

| Critério | Avaliação |
|---------|-----------|
| Uso de Minimal API | Correto para o porte da aplicação |
| Source Generators JSON | Boa prática para AOT |
| Handlers não conectados aos endpoints | Bloqueante — API não funcional |
| `AddApplicationHandlers()` não chamado | Bloqueante |
| `AddInfrastructureServices()` não chamado | Bloqueante |
| JWT configurado mas não aplicado nos endpoints | Incompleto |
| `JwtSettings` ausente em produção | Bug de startup em produção |
| `RequireHttpsMetadata = false` | Risco de segurança |
| `FindGamesQueryResponse` fora do JsonContext | Erro em runtime se endpoint for ativado |
| Delete endpoint ausente | Funcionalidade não implementada |
