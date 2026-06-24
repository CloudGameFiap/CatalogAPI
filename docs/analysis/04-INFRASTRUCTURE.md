# Análise da Camada de Infraestrutura

**Projeto:** `CloudGameCatalog.Infrastructure`
**Dependências:** `CloudGameCatalog.Domain`, EF Core, Dapper, SQL Server

---

## Estrutura de Pastas

```
Infrastructure/
├── EntityFramework/
│   ├── AppDbContext.cs
│   ├── UnitOfWork.cs
│   ├── Mappings/
│   │   └── GameMapping.cs
│   └── Repositories/
│       ├── AbstractRepository.cs   → Base genérica EF Core (write)
│       └── GameWriteOnlyRepository.cs
├── Dapper/
│   ├── Contracts/
│   │   └── IDapperContext.cs
│   ├── DapperContext.cs
│   └── Repositories/
│       ├── AbstractRepository.cs   → Base genérica Dapper (read)
│       └── GameReadOnlyRepository.cs
└── Extensions/
    └── InfrastructureExtensions.cs
```

---

## Entity Framework Core (Write Path)

### AppDbContext

```csharp
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Game> Games { get; set; }
    // Aplica mapeamentos via IEntityTypeConfiguration
}
```

### GameMapping

```csharp
builder.ToTable("Games");
builder.HasKey(k => k.Id);
builder.Property(p => p.Name).IsRequired().HasColumnType("VARCHAR(120)");
builder.Property(p => p.Description).IsRequired().HasColumnType("VARCHAR(120)");
builder.Property(p => p.ImageUrl).IsRequired().HasColumnType("VARCHAR(250)");
builder.Property(p => p.Genre).IsRequired().HasColumnType("VARCHAR(60)");
builder.Property(p => p.CreatedAt).HasColumnType("DATETIME2");
builder.Property(p => p.ReleaseDate).HasColumnType("DATETIME2");
builder.Property(p => p.Active).HasColumnType("BIT");
builder.Property(p => p.Price).HasColumnType("DECIMAL(18,2)");
```

> **Inconsistência:** `ImageUrl` é `string?` na entidade (nullable) mas mapeado como `.IsRequired()` no EF Core.

### Schema resultante da tabela Games

```sql
Games (
    Id          INT           PRIMARY KEY IDENTITY,
    Name        VARCHAR(120)  NOT NULL,
    Description VARCHAR(120)  NOT NULL,
    ImageUrl    VARCHAR(250)  NOT NULL,
    Genre       VARCHAR(60)   NOT NULL,
    Price       DECIMAL(18,2) NOT NULL,
    Active      BIT           NOT NULL,
    CreatedAt   DATETIME2     NOT NULL,
    ReleaseDate DATETIME2     NOT NULL
)
```

### Repositório EF Core (Write) — AbstractRepository

```csharp
// Operações genéricas via DbContext
AddAsync(entity)    → context.Set<TEntity>().AddAsync(entity)
UpdateAsync(entity) → context.Set<TEntity>().Update(entity)
RemoveAsync(entity) → context.Set<TEntity>().Remove(entity)
```

### UnitOfWork

```csharp
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task BeginTransactionAsync() → context.Database.BeginTransactionAsync()
    public async Task<int> SaveChangesAsync() → context.SaveChangesAsync()
}
```

> `BeginTransactionAsync()` está disponível no contrato mas **nenhum handler o chama** — as operações de escrita salvam diretamente sem transação explícita.

---

## Dapper (Read Path)

### IDapperContext / DapperContext

```csharp
public interface IDapperContext
{
    IDbConnection OpenConnection();
}

public class DapperContext(IConfiguration configuration) : IDapperContext
{
    public IDbConnection OpenConnection()
        => new SqlConnection(configuration.GetConnectionString("Default"));
}
```

Fábrica de conexões. Cada repositório cria e descarta sua própria conexão (`using IDbConnection`).

### Repositório Dapper (Read) — AbstractRepository

Usa `Dapper.Contrib` para busca por Id:

```csharp
public async Task<TEntity?> GetByIdAsync(TId id)
{
    using IDbConnection connection = Context.OpenConnection();
    return await connection.GetAsync<TEntity>(id);
}
```

### GameReadOnlyRepository — FindAsync

```csharp
public async Task<Pagination<Game>> FindAsync(FindGamesParameter parameters)
{
    var sqlBuilder = new SqlBuilder();

    if (parameters.Active.HasValue)
        sqlBuilder.Where("Active=@active", new { active = parameters.Active });

    if (!string.IsNullOrWhiteSpace(parameters.Name))
        sqlBuilder.Where("Name LIKE @name", new { name = $"%{parameters.Name}%" });

    var countQuery = sqlBuilder.AddTemplate("Select count(*) from Game /**where**/");
    var queryGames = sqlBuilder.AddTemplate(@"
        select Id, Name, Active from Game
        /**where**/ ORDER BY Id OFFSET @skip ROWS FETCH NEXT @size ROWS ONLY",
        new { skip = parameters.Skip, size = parameters.PageSize });

    using IDbConnection connection = Context.OpenConnection();
    int count  = await connection.ExecuteScalarAsync<int>(countQuery.RawSql, countQuery.Parameters);
    var games  = (await connection.QueryAsync<Game>(queryGames.RawSql, queryGames.Parameters)).ToList();

    return new Pagination<Game>(games, count);
}
```

**Pontos de atenção:**
- A query usa `from Game` (nome do tipo C#) e não `from Games` (nome da tabela). Isso funciona com Dapper.Contrib mas pode falhar em algumas configurações de SQL Server — precisa de verificação.
- São executadas **duas queries** por chamada (count + data) — adequado para paginação.
- Parâmetros são passados com `@` — seguros contra SQL injection.

---

## Pacotes NuGet Utilizados

| Pacote | Versão | Uso |
|--------|--------|-----|
| `Microsoft.EntityFrameworkCore` | 10.0.9 | ORM principal |
| `Microsoft.EntityFrameworkCore.SqlServer` | 10.0.9 | Provider SQL Server |
| `Microsoft.EntityFrameworkCore.Tools` | 10.0.9 | Migrations (build-time) |
| `Dapper` | 2.1.79 | Micro-ORM para leitura |
| `Dapper.Contrib` | 2.0.78 | GetAsync por Id |
| `Dapper.SqlBuilder` | 2.1.66 | Queries dinâmicas com `/**where**/` |

---

## Registro no DI

```csharp
// InfrastructureExtensions.cs
services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connString));
services.AddScoped<IDbConnection>(sp => new SqlConnection(connString));
services.AddScoped<IDapperContext>(sp => new DapperContext(configuration));
services.AddScoped<IGameWriteOnlyRepository, GameWriteOnlyRepository>();
services.AddScoped<IGameReadOnlyRepository, GameReadOnlyRepository>();
services.AddScoped<IUnitOfWork, UnitOfWork>(sp => new UnitOfWork(sp.GetRequiredService<AppDbContext>()));
```

> `AddInfrastructureServices()` **não é chamado** em `Program.cs` atualmente.

---

## Migrations

Não foram encontrados arquivos de migration na solution. O schema precisaria ser criado manualmente ou via `dotnet ef migrations add Initial`.

---

## Avaliação da Camada de Infraestrutura

| Critério | Avaliação |
|---------|-----------|
| Separação EF Core (write) / Dapper (read) | Correto |
| Mapeamento Fluent API | Correto |
| Inconsistência ImageUrl (nullable vs IsRequired) | Problema |
| Nome da tabela no Dapper query (`Game` vs `Games`) | Risco de erro em runtime |
| Proteção contra SQL injection | Correto (parâmetros) |
| Migrations ausentes | Pendente |
| `BeginTransactionAsync` nunca chamado | Funcionalidade morta |
| Connection string em dois lugares (DI: IDbConnection + DapperContext) | Redundante |
| `AddInfrastructureServices` não chamado no Program.cs | Bloqueante |
