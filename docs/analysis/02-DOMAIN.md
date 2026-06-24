# Análise da Camada de Domínio

**Projeto:** `CloudGameCatalog.Domain`
**Dependências externas:** Nenhuma (domínio puro)

---

## Estrutura de Pastas

```
Domain/
├── Entities/
│   ├── Entity.cs               → Base genérica com Id e CreatedAt
│   └── Game.cs                 → Aggregate root do contexto
├── Commom/
│   ├── Error.cs                → Value object de erro (record struct)
│   ├── Result.cs               → Result pattern (Result e Result<T>)
│   ├── Pagination.cs           → Wrapper de lista paginada
│   └── PaginationParameters.cs → Parâmetros de paginação base
├── Parameters/
│   └── FindGamesParameter.cs   → Parâmetro de busca com filtros
├── Handlers/
│   ├── ICommand.cs             → Marker interface para comandos
│   ├── IResponse.cs            → Marker interface para respostas
│   └── IHandler.cs             → Contrato dos handlers CQRS
└── Interfaces/
    ├── IReadOnlyRepository.cs
    ├── IWriteOnlyRepository.cs
    ├── IGameReadOnlyRepository.cs
    ├── IGameWriteOnlyRepository.cs
    └── IUnitOfWork.cs
```

---

## Entidade Base

```csharp
// Entity<TId>
public abstract class Entity<TId>
{
    public TId Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
}
```

Permite que o Id seja um tipo genérico — `Game` usa `int`.

---

## Entidade Game (Aggregate Root)

```csharp
public sealed class Game : Entity<int>
{
    public string Name        { get; private set; }
    public string Description { get; private set; }
    public string? ImageUrl   { get; private set; }
    public decimal Price      { get; private set; }
    public string Genre       { get; private set; }
    public DateTime ReleaseDate { get; private set; }
    public bool Active        { get; private set; }
}
```

### Comportamentos

| Método | Descrição |
|--------|-----------|
| `Game(...)` | Construtor público que seta `Active = true` e chama `Validate()` |
| `Update(...)` | Atualiza todos os campos e re-valida |
| `Validate()` | Auto-validação com `ArgumentException` |
| `private Game()` | Construtor privado para EF Core |

### Validações no Domínio (`Validate()`)

| Campo | Regra |
|-------|-------|
| `Name` | Não pode ser nulo ou vazio |
| `Description` | Não pode ser nulo ou vazio |
| `ImageUrl` | Não pode ser nulo ou vazio (lança exceção mesmo sendo `string?`) |
| `Genre` | Não pode ser nulo ou vazio |
| `Price` | Não pode ser negativo |

> **Observação:** `ImageUrl` é declarado como `string?` na entidade, mas `Validate()` lança exceção quando nulo/vazio — há uma inconsistência semântica entre o tipo nullable e a validação obrigatória.

---

## Value Objects

### Error

```csharp
public readonly record struct Error(string Code, string Description);
```

- Imutável (`readonly record struct`)
- Comparação por valor automática
- Exemplos de uso: `new Error("NotFound", "Jogo não encontrado")`

### Result / Result\<T\>

```csharp
public class Result
{
    public IReadOnlyList<Error> Errors { get; }
    public bool IsSuccess => Errors.Count == 0;
    public static Result Success() => new([]);
    public static Result Failure(List<Error> errors) => new(errors);
}

public sealed class Result<T> : Result where T : notnull
{
    public T? Data { get; }
    public static Result<T> Success(T data) => new(data);
    public static new Result<T> Failure(List<Error> errors) => new(errors);
}
```

Evita o uso de exceções para controle de fluxo de negócio. Retorno uniforme em todos os handlers.

---

## Paginação

### PaginationParameters

```csharp
public class PaginationParameters
{
    public int PageNumber { get; set; } = 1;
    public int PageSize   { get; set; } = 10;
    public int Skip       => (PageNumber - 1) * PageSize;  // calculado
}
```

### FindGamesParameter (extends PaginationParameters)

```csharp
public class FindGamesParameter : PaginationParameters
{
    public string? Name   { get; set; }
    public bool? Active   { get; set; }
}
```

Filtros opcionais que o repositório aplica condicionalmente.

### Pagination\<T\>

```csharp
public class Pagination<TResult>
{
    public IReadOnlyList<TResult> Items { get; }
    public int Count { get; }
}
```

Wrapper simples para retornar lista + total de registros.

---

## Interfaces de Repositório

### Genéricas

```csharp
IReadOnlyRepository<TEntity, TId>
    GetByIdAsync(TId id) → Task<TEntity?>

IWriteOnlyRepository<TEntity, TId>
    AddAsync(TEntity entity) → Task
    UpdateAsync(TEntity entity) → Task
    RemoveAsync(TEntity entity) → Task
```

### Específicas para Game

```csharp
IGameReadOnlyRepository : IReadOnlyRepository<Game, int>
    FindAsync(FindGamesParameter) → Task<Pagination<Game>>

IGameWriteOnlyRepository : IWriteOnlyRepository<Game, int>
    // Herda AddAsync, UpdateAsync, RemoveAsync
```

---

## Interfaces de Handler CQRS

```csharp
IHandler<TCommand, TResponse>
    where TCommand : ICommand, new()
    where TResponse : IResponse
{
    Task<Result<TResponse>> HandleAsync(TCommand, CancellationToken);
}

IHandler<TCommand>
    where TCommand : ICommand, new()
{
    Task<Result> HandleAsync(TCommand, CancellationToken);
}
```

A constraint `new()` nos commands exige construtor sem parâmetros — necessário para deserialização JSON nos Minimal APIs.

---

## Interface Unit of Work

```csharp
IUnitOfWork
    BeginTransactionAsync() → Task
    SaveChangesAsync() → Task<int>
```

---

## Avaliação da Camada de Domínio

| Critério | Avaliação |
|---------|-----------|
| Isolamento de dependências | Excelente — zero dependências externas |
| Encapsulamento da entidade | Bom — setters privados |
| Auto-validação da entidade | Presente, mas usa exceções em vez de Result |
| Consistência do tipo ImageUrl (`string?` vs obrigatorio) | Inconsistente |
| Value objects imutáveis | Correto (`Error` como record struct) |
| Interfaces bem definidas | Correto |
| Typo no namespace | `Commom` deveria ser `Common` |
