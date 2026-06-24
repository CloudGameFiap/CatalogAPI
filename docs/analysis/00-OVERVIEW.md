# CloudGameFiap — CatalogAPI: Visão Geral

## Identificação do Projeto

| Item | Detalhe |
|------|---------|
| Nome | CloudGameFiap — CatalogAPI |
| Contexto | Tech Challenge FIAP |
| Framework | .NET 10.0 |
| Tipo | Microserviço REST (ASP.NET Core Minimal API) |
| Banco de Dados | SQL Server 2022 |
| Containerização | Docker / Docker Compose |

---

## Estrutura da Solution

```
CloudGameCatalog.slnx
└── src/
    ├── CloudGameCatalog.Api            → Camada de apresentação (Minimal API)
    ├── CloudGameCatalog.Application    → Casos de uso (CQRS handlers + validadores)
    ├── CloudGameCatalog.Domain         → Entidades, interfaces, contratos
    └── CloudGameCatalog.Infrastructure → Persistência (EF Core + Dapper)
```

---

## Grafo de Dependências

```
Api ──► Application ──► Domain
                            ▲
Infrastructure ─────────────┘
```

- **Domain** não depende de ninguém (núcleo puro).
- **Infrastructure** conhece apenas Domain (inversão de dependência via interfaces).
- **Application** conhece Domain; não conhece Infrastructure.
- **Api** conhece Application (injeta handlers via DI).

---

## Padrões Arquiteturais Identificados

| Padrão | Onde Aplicado |
|--------|--------------|
| Clean Architecture | Separação em 4 camadas com dependências unidirecionais |
| CQRS | Commands (Create/Update) separados de Queries (GetById/Find) |
| Repository Pattern | `IGameReadOnlyRepository` / `IGameWriteOnlyRepository` |
| Unit of Work | `IUnitOfWork` gerencia transações EF Core |
| Result Pattern | `Result<T>` para retorno funcional sem exceções |
| Hybrid ORM | EF Core para escrita, Dapper para leitura |
| Minimal API | Endpoints declarados diretamente em `Program.cs` |
| Source Generators | `AppJsonSerializerContext` para serialização AOT-friendly |

---

## Índice dos Documentos de Análise

| Arquivo | Conteúdo |
|---------|---------|
| [01-ARCHITECTURE.md](01-ARCHITECTURE.md) | Análise da arquitetura e padrões |
| [02-DOMAIN.md](02-DOMAIN.md) | Camada de domínio — entidades, contratos, value objects |
| [03-APPLICATION.md](03-APPLICATION.md) | Camada de aplicação — handlers, validadores, CQRS |
| [04-INFRASTRUCTURE.md](04-INFRASTRUCTURE.md) | Infraestrutura — EF Core, Dapper, repositórios |
| [05-API.md](05-API.md) | Camada de API — endpoints, autenticação, serialização |
| [06-DOCKER.md](06-DOCKER.md) | Docker e Docker Compose |
| [07-ISSUES.md](07-ISSUES.md) | Problemas encontrados e recomendações |
