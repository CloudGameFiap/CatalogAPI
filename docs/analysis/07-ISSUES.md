# Problemas Encontrados e Recomendações

Classificação de severidade: **Bloqueante** | **Alta** | **Média** | **Baixa**

---

## Bloqueantes — A API não funciona em produção

### 1. Handlers não conectados aos endpoints

**Arquivo:** [Program.cs](../../src/CloudGameCatalog.Api/Program.cs)

Todos os endpoints retornam dados hardcoded. Os handlers CQRS foram implementados corretamente mas não são chamados.

```csharp
// Atual — dados fixos
gamesApi.MapPost("/", (CreateGameCommand command) =>
    TypedResults.Ok(new CreateGameCommandResponse(0, "teste", true)));

// Esperado — chamar o handler via DI
gamesApi.MapPost("/", async (
    CreateGameCommand command,
    IHandler<CreateGameCommand, CreateGameCommandResponse> handler,
    CancellationToken ct) =>
{
    var result = await handler.HandleAsync(command, ct);
    return result.IsSuccess
        ? Results.Ok(result.Data)
        : Results.BadRequest(result.Errors);
});
```

---

### 2. `AddApplicationHandlers()` e `AddInfrastructureServices()` não chamados

**Arquivo:** [Program.cs](../../src/CloudGameCatalog.Api/Program.cs)

Os métodos de extensão existem e estão corretos, mas nunca são chamados no startup.

```csharp
// Ausente no Program.cs:
builder.Services.AddApplicationHandlers();
builder.Services.AddInfrastructureServices(builder.Configuration);
```

---

### 3. `JwtSettings` ausente no `appsettings.json` de produção

**Arquivo:** [appsettings.json](../../src/CloudGameCatalog.Api/appsettings.json)

`Program.cs` usa `GetRequiredSection("JwtSettings")` que lança `InvalidOperationException` se a seção não existir. A seção só existe em `appsettings.Development.json`.

**Solução:** Adicionar `JwtSettings` ao `appsettings.json` com valores padrão ou injetar via variáveis de ambiente no Docker Compose.

---

### 4. Migrations ausentes

O schema do banco não é criado automaticamente. Não há arquivos de migration na solution.

**Solução:** Executar `dotnet ef migrations add Initial` e chamar `dbContext.Database.MigrateAsync()` no startup, ou criar um script SQL de inicialização para o Docker.

---

## Alta Severidade

### 5. Senha SA do SQL Server hardcoded no docker-compose.yml

```yaml
MSSQL_SA_PASSWORD: "CloudGame#2026"
```

Credenciais de banco em controle de versão. Deve ser externalizado para `.env` ou Docker secrets.

```yaml
# .env (fora do git — adicionar ao .gitignore)
MSSQL_SA_PASSWORD=CloudGame#2026

# docker-compose.yml
environment:
  MSSQL_SA_PASSWORD: ${MSSQL_SA_PASSWORD}
```

---

### 6. Validators do FluentValidation não são executados

**Arquivos:** [CreateGameCommandHandler.cs](../../src/CloudGameCatalog.Application/Handlers/GameHandler/Create/CreateGameCommandHandler.cs), [UpdateGameCommandHandler.cs](../../src/CloudGameCatalog.Application/Handlers/GameHandler/Update/UpdateGameCommandHandler.cs)

Os validators existem (`CreateGameCommandValidator`, `UpdateGameCommandValidator`) mas nunca são chamados pelos handlers. Requisições inválidas são processadas até atingir a validação da entidade, que lança `ArgumentException` não tratada.

**Solução:** Registrar validators no DI e chamar antes do handler:

```csharp
// No handler ou em um pipeline behavior
var validationResult = await validator.ValidateAsync(command, cancellationToken);
if (!validationResult.IsValid)
    return Result<T>.Failure(validationResult.Errors.Select(e => new Error("Validation", e.ErrorMessage)).ToList());
```

---

### 7. `RequireHttpsMetadata = false` em produção

**Arquivo:** [Program.cs](../../src/CloudGameCatalog.Api/Program.cs) linha 35

JWT Bearer aceita tokens via HTTP puro. Isso permite interceptação de tokens.

```csharp
// Atual
x.RequireHttpsMetadata = false;

// Correto para produção
x.RequireHttpsMetadata = !app.Environment.IsDevelopment();
```

---

### 8. Autenticação não aplicada em nenhum endpoint

JWT está configurado mas nenhum endpoint usa `.RequireAuthorization()`. Todos os endpoints são públicos.

```csharp
// Adicionar por endpoint
gamesApi.MapPost("/", ...).RequireAuthorization();

// Ou aplicar a todo o grupo
var gamesApi = app.MapGroup("/games").RequireAuthorization();
```

---

## Média Severidade

### 9. Nome da tabela divergente no Dapper

**Arquivo:** [GameReadOnlyRepository.cs](../../src/CloudGameCatalog.Infrastructure/Dapper/Repositories/GameReadOnlyRepository.cs)

```sql
-- Query usa:
Select count(*) from Game

-- Mas a tabela se chama (conforme GameMapping.cs):
builder.ToTable("Games");
```

O nome `Game` pode funcionar em alguns contextos do SQL Server por coincidência mas é um bug latente.

---

### 10. `ImageUrl` inconsistente — nullable vs obrigatório

**Domain:** `string? ImageUrl` (nullable)
**GameMapping.cs:** `.IsRequired()` (não nullable no banco)
**Game.Validate():** Lança exceção se nulo/vazio

Três camadas com comportamento diferente para o mesmo campo. Deve ser padronizado.

---

### 11. `IDbConnection` registrado duas vezes no DI

**Arquivo:** [InfrastructureExtensions.cs](../../src/CloudGameCatalog.Infrastructure/Extensions/InfrastructureExtensions.cs)

```csharp
services.AddScoped<IDbConnection>(sp => new SqlConnection(...));  // linha 26
services.AddScoped<IDapperContext>(sp => new DapperContext(...)); // linha 27
// DapperContext internamente também cria SqlConnection
```

`IDbConnection` é registrado mas nunca usado (repositórios usam `IDapperContext`). Registro redundante.

---

### 12. `BeginTransactionAsync()` nunca chamado

**Arquivo:** [IUnitOfWork.cs](../../src/CloudGameCatalog.Domain/Interfaces/IUnitOfWork.cs)

O contrato declara `BeginTransactionAsync()` e a implementação existe, mas nenhum handler a chama. Operações de escrita rodam sem transação explícita.

---

### 13. Container da API roda como root

O Dockerfile não define um usuário não-root. Em produção, recomenda-se:

```dockerfile
RUN adduser --disabled-password --gecos "" appuser
USER appuser
```

---

### 14. `.dockerignore` ausente

Builds Docker copiam `bin/`, `obj/`, `.vs/`, `.git/` desnecessariamente, tornando o contexto de build maior e o processo mais lento.

---

## Baixa Severidade

### 15. Typo: `Commom` → deveria ser `Common`

**Namespace:** `CloudGameCatalog.Domain.Commom`

Afeta todos os arquivos que importam esse namespace (`Error.cs`, `Result.cs`, `Pagination.cs`, `PaginationParameters.cs`).

---

### 16. Typo: `EncrictKey` / `EncriptKey`

**appsettings.Development.json:** `"EncrictKey"`
**JwtSettings.cs:** `"EncriptKey"`
**Program.cs:** `GetValue<string>("EncrictKey")`

Três grafias diferentes para a mesma chave. `EncryptKey` seria o correto.

---

### 17. `FindGamesQueryResponse` não registrado no `AppJsonSerializerContext`

**Arquivo:** [Program.cs](../../src/CloudGameCatalog.Api/Program.cs)

Se o endpoint de listagem for conectado ao handler real, `Pagination<FindGamesQueryResponse>` não terá suporte de serialização AOT e poderá falhar em runtime (dependendo do modo de publicação).

---

### 18. Endpoint DELETE ausente

Nenhum endpoint de exclusão de jogo foi implementado. Pode ser intencional para a fase atual.

---

### 19. Mensagens de erro misturando português e inglês

```csharp
// Em inglês:
throw new ArgumentException("Game name cannot be empty or null.");

// Em português:
return Result.Failure([new Error("NotFound", "Jogo não encontrado")]);
```

Padronizar o idioma das mensagens de erro.

---

### 20. Sem testes automatizados

Nenhum projeto de testes encontrado na solution. Para um Tech Challenge FIAP, testes unitários dos handlers e integração dos repositórios seriam esperados.

---

## Resumo por Prioridade

| # | Problema | Severidade | Arquivo Principal |
|---|---------|-----------|------------------|
| 1 | Handlers não conectados aos endpoints | Bloqueante | Program.cs |
| 2 | AddApplicationHandlers/AddInfrastructureServices não chamados | Bloqueante | Program.cs |
| 3 | JwtSettings ausente em produção | Bloqueante | appsettings.json |
| 4 | Migrations ausentes | Bloqueante | — |
| 5 | Senha SA hardcoded | Alta | docker-compose.yml |
| 6 | Validators não executados | Alta | Handlers |
| 7 | `RequireHttpsMetadata = false` em produção | Alta | Program.cs |
| 8 | Endpoints sem autenticação | Alta | Program.cs |
| 9 | Nome tabela divergente Dapper (`Game` vs `Games`) | Média | GameReadOnlyRepository.cs |
| 10 | `ImageUrl` inconsistente | Média | Game.cs / GameMapping.cs |
| 11 | `IDbConnection` registrado redundantemente | Média | InfrastructureExtensions.cs |
| 12 | `BeginTransactionAsync` nunca chamado | Média | Handlers |
| 13 | Container roda como root | Média | Dockerfile |
| 14 | `.dockerignore` ausente | Média | — |
| 15 | Typo `Commom` | Baixa | Domain/Commom/ |
| 16 | Typo `EncrictKey`/`EncriptKey` | Baixa | JwtSettings / appsettings |
| 17 | `FindGamesQueryResponse` fora do JsonContext | Baixa | Program.cs |
| 18 | Endpoint DELETE ausente | Baixa | Program.cs |
| 19 | Mensagens de erro em idiomas mistos | Baixa | Game.cs / Handlers |
| 20 | Sem testes automatizados | Baixa | — |
