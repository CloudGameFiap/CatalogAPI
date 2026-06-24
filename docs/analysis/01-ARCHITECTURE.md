# Análise de Arquitetura

## Clean Architecture

O projeto segue Clean Architecture com 4 camadas bem definidas. A regra da dependência é respeitada: as camadas internas não conhecem as externas.

```
┌─────────────────────────────────┐
│            API Layer            │  ← Minimal API, JWT, Serialização
├─────────────────────────────────┤
│       Application Layer         │  ← Handlers CQRS, Validators
├─────────────────────────────────┤
│         Domain Layer            │  ← Entidades, Interfaces, Value Objects
├─────────────────────────────────┤
│      Infrastructure Layer       │  ← EF Core, Dapper, Repositórios
└─────────────────────────────────┘
```

---

## CQRS (Command Query Responsibility Segregation)

Comandos e queries estão separados em namespaces distintos, com handlers, contracts e responses próprios.

### Comandos (escrita)

| Comando | Handler | Repositório |
|---------|---------|-------------|
| `CreateGameCommand` | `CreateGameCommandHandler` | `IGameWriteOnlyRepository` + `IUnitOfWork` |
| `UpdateGameCommand` | `UpdateGameCommandHandler` | `IGameWriteOnlyRepository` + `IUnitOfWork` |

### Queries (leitura)

| Query | Handler | Repositório |
|-------|---------|-------------|
| `GetGameByIdQuery` | `GetGameByIdQueryHandler` | `IGameReadOnlyRepository` |
| `FindGamesQuery` | `FindGamesQueryHandler` | `IGameReadOnlyRepository` |

---

## Hybrid ORM — EF Core (Write) + Dapper (Read)

Escolha deliberada para separar performance de leitura da segurança de escrita:

```
Write Path:  Command → Handler → EF Core Repository → UnitOfWork → SQL Server
Read Path:   Query  → Handler → Dapper Repository  → SQL Server
```

| Aspecto | EF Core | Dapper |
|---------|---------|--------|
| Uso | Inserção e atualização | Consultas com filtro e paginação |
| Rastreamento | Change Tracker ativo | Sem rastreamento (somente leitura) |
| Query | LINQ / Fluent API | SQL nativo + `SqlBuilder` |
| Flexibilidade | Menor | Maior para queries complexas |

---

## Inversão de Dependência (DIP)

Infrastructure implementa interfaces definidas no Domain, nunca o contrário:

```
Domain:         IGameReadOnlyRepository   IGameWriteOnlyRepository   IUnitOfWork
Infrastructure: GameReadOnlyRepository    GameWriteOnlyRepository    UnitOfWork
```

Registro no DI container feito em `InfrastructureExtensions.cs` e `ApplicationExtensions.cs`.

---

## Result Pattern

Toda operação de handler retorna `Result<T>` em vez de lançar exceções para fluxos de negócio:

```csharp
// Sucesso
Result<CreateGameCommandResponse>.Success(response)

// Falha de negócio
Result<UpdateGameCommandResponse>.Failure([new Error("NotFound", "Jogo não encontrado")])
```

`Error` é um `record struct` imutável com `Code` e `Description`.

---

## Fluxo Completo de uma Requisição

```
HTTP Request
    │
    ▼
[Program.cs — Minimal API endpoint]
    │
    ▼
[IHandler<TCommand, TResponse>.HandleAsync()]
    │
    ├──► [FluentValidation Validator] (validação do command)
    │
    ├──► [IGameWriteOnlyRepository / IGameReadOnlyRepository]
    │         │
    │         ├── EF Core (escrita)
    │         └── Dapper (leitura)
    │
    ├──► [IUnitOfWork.SaveChangesAsync()] (se escrita)
    │
    └──► Result<TResponse>
              │
              ▼
         HTTP Response
```

---

## Registro de Dependências

### Application (`ApplicationExtensions.cs`)

```csharp
services.AddScoped<IHandler<CreateGameCommand, CreateGameCommandResponse>, CreateGameCommandHandler>();
services.AddScoped<IHandler<UpdateGameCommand, UpdateGameCommandResponse>, UpdateGameCommandHandler>();
services.AddScoped<IHandler<GetGameByIdQuery, GetGameByIdResponse>, GetGameByIdQueryHandler>();
services.AddScoped<IHandler<FindGamesQuery, Pagination<FindGamesQueryResponse>>, FindGamesQueryHandler>();
```

### Infrastructure (`InfrastructureExtensions.cs`)

```csharp
services.AddDbContext<AppDbContext>(options => options.UseSqlServer(...));
services.AddScoped<IDbConnection>(sp => new SqlConnection(...));
services.AddScoped<IDapperContext>(sp => new DapperContext(configuration));
services.AddScoped<IGameWriteOnlyRepository, GameWriteOnlyRepository>();
services.AddScoped<IGameReadOnlyRepository, GameReadOnlyRepository>();
services.AddScoped<IUnitOfWork, UnitOfWork>(...);
```

> **Observação:** `AddApplicationHandlers()` e `AddInfrastructureServices()` **não são chamados** em `Program.cs` na versão atual — os handlers estão registrados mas os endpoints usam dados hardcoded.

---

## Avaliação Geral da Arquitetura

| Critério | Avaliação |
|---------|-----------|
| Separação de responsabilidades | Excelente |
| Inversão de dependência | Correto |
| CQRS aplicado | Correto |
| Result Pattern | Bem implementado |
| Handlers integrados aos endpoints | Pendente |
| Validação integrada aos handlers | Pendente |
| Testes unitários/integração | Ausentes |
