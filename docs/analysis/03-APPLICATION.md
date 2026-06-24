# Análise da Camada de Aplicação

**Projeto:** `CloudGameCatalog.Application`
**Dependências:** `CloudGameCatalog.Domain`, `FluentValidation`, `Microsoft.Extensions.DependencyInjection.Abstractions`

---

## Estrutura de Pastas

```
Application/
├── Extensions/
│   └── ApplicationExtensions.cs    → Registro de handlers no DI
├── Settings/
│   └── JwtSettings.cs              → POCO para configuração JWT
└── Handlers/
    └── GameHandler/
        ├── Create/
        │   ├── CreateGameCommand.cs
        │   ├── CreateGameCommandHandler.cs
        │   ├── CreateGameCommandResponse.cs
        │   └── CreateGameCommandValidator.cs
        ├── Update/
        │   ├── UpdateGameCommand.cs
        │   ├── UpdateGameCommandHandler.cs
        │   ├── UpdateGameCommandResponse.cs
        │   └── UpdateGameCommandValidator.cs
        ├── GetById/
        │   ├── GetGameByIdQuery.cs
        │   ├── GetGameByIdQueryHandler.cs
        │   └── GetGameByIdResponse.cs
        └── Find/
            ├── FindGamesQuery.cs
            ├── FindGamesQueryHandler.cs
            └── FindGamesQueryResponse.cs
```

---

## Handlers — Comandos

### CreateGame

**Command:**
```csharp
public record CreateGameCommand : ICommand
{
    public string Name        { get; set; }
    public string Description { get; set; }
    public string? ImageUrl   { get; set; }
    public decimal Price      { get; set; }
    public string Genre       { get; set; }
    public DateTime ReleaseDate { get; set; }
}
```

**Handler — fluxo:**
1. Instancia `Game` com os dados do command (construtor chama `Validate()`)
2. `gameWriteOnlyRepository.AddAsync(newGame)`
3. `unitOfWork.SaveChangesAsync()`
4. Retorna `Result<CreateGameCommandResponse>.Success(...)`

**Response:**
```csharp
public record CreateGameCommandResponse(int Id, string Name, bool Active) : IResponse;
```

**Validator (FluentValidation):**
```
Name: NotEmpty + MaximumLength(100)
```

> Os demais campos (`Description`, `Genre`, `Price`, `ReleaseDate`) **não são validados** pelo validator da camada de Application — apenas pela entidade via `ArgumentException`.

---

### UpdateGame

**Command:**
```csharp
public record UpdateGameCommand : ICommand
{
    public int Id             { get; set; }
    public string Name        { get; set; }
    // ... mesmos campos do Create
}
```

**Handler — fluxo:**
1. `gameReadOnlyRepository.GetByIdAsync(command.Id)`
2. Se não encontrado → `Result.Failure([Error("NotFound", ...)])`
3. `gameToUpdate.Update(...)` (chama `Validate()` internamente)
4. `gameWriteOnlyRepository.UpdateAsync(gameToUpdate)`
5. `unitOfWork.SaveChangesAsync()`
6. Retorna `Result<UpdateGameCommandResponse>.Success(...)`

**Validator (FluentValidation):**
```
Id:   NotEmpty
Name: NotEmpty + MaximumLength(100)
```

---

## Handlers — Queries

### GetGameById

**Query:** `record GetGameByIdQuery(int Id) : ICommand`

**Handler — fluxo:**
1. `gameReadOnlyRepository.GetByIdAsync(request.Id)`
2. Se não encontrado → `Result.Failure([Error("NotFound", ...)])`
3. Mapeia `Game` → `GetGameByIdResponse` e retorna Success

**Response:**
```csharp
public record GetGameByIdResponse(
    int Id, string Name, string Description,
    string? ImageUrl, decimal Price, string Genre,
    DateTime ReleaseDate, bool Active
) : IResponse;
```

---

### FindGames

**Query:** `record FindGamesQuery : ICommand` (herda `FindGamesParameter`)

**Handler — fluxo:**
1. `gameReadOnlyRepository.FindAsync(query)` → `Pagination<Game>`
2. Projeta `Game` → `FindGamesQueryResponse` para cada item
3. Retorna `Result<Pagination<FindGamesQueryResponse>>.Success(...)`

**Response:**
```csharp
public record FindGamesQueryResponse(int Id, string Name, bool Active) : IResponse;
```

> A query de listagem retorna apenas `Id`, `Name` e `Active` — resposta enxuta adequada para listas.

---

## Validadores (FluentValidation)

| Validator | Campo | Regras |
|-----------|-------|--------|
| `CreateGameCommandValidator` | `Name` | NotEmpty, MaxLength(100) |
| `UpdateGameCommandValidator` | `Id` | NotEmpty |
| `UpdateGameCommandValidator` | `Name` | NotEmpty, MaxLength(100) |

**Problema:** Os validators **não são invocados pelos handlers**. Não há chamada a `validator.ValidateAsync()` dentro dos handlers nem pipeline de validação registrado.

---

## Settings

```csharp
public class JwtSettings
{
    public string EncriptKey       { get; set; }
    public string Audience         { get; set; }
    public string Issuer           { get; set; }
    public int ExpiresInMinutes    { get; set; }
}
```

- Lida via `IOptions<JwtSettings>` (configurado em `Program.cs`)
- Chave configurada apenas em `appsettings.Development.json`
- **Typo:** `EncriptKey` deveria ser `EncryptKey`

---

## Registro no DI

```csharp
// ApplicationExtensions.cs
services.AddScoped<IHandler<CreateGameCommand, CreateGameCommandResponse>, CreateGameCommandHandler>();
services.AddScoped<IHandler<UpdateGameCommand, UpdateGameCommandResponse>, UpdateGameCommandHandler>();
services.AddScoped<IHandler<GetGameByIdQuery, GetGameByIdResponse>, GetGameByIdQueryHandler>();
services.AddScoped<IHandler<FindGamesQuery, Pagination<FindGamesQueryResponse>>, FindGamesQueryHandler>();
```

> `AddApplicationHandlers()` **não é chamado** em `Program.cs`. Os handlers estão implementados mas não chegam a ser utilizados pelos endpoints ainda.

---

## Avaliação da Camada de Aplicação

| Critério | Avaliação |
|---------|-----------|
| CQRS com handlers explícitos | Correto |
| Uso do Result Pattern | Correto |
| Validators criados | Sim |
| Validators executados nos handlers | Não — ausente |
| Validators registrados no DI | Não |
| Handlers registrados no DI | Sim (mas não chamados nos endpoints) |
| Mapeamento entity → response | Manual (sem AutoMapper — OK para este porte) |
| Typo em `EncriptKey` | Presente |
