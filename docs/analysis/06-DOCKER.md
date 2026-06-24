# Análise de Docker e Infraestrutura

---

## Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/CloudGameCatalog.Api/CloudGameCatalog.Api.csproj", "src/CloudGameCatalog.Api/"]
# ... demais projetos
RUN dotnet restore "src/CloudGameCatalog.Api/CloudGameCatalog.Api.csproj"
COPY . .
RUN dotnet build ...
RUN dotnet publish ... -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CloudGameCatalog.Api.dll"]
```

### Avaliação do Dockerfile

| Critério | Avaliação |
|---------|-----------|
| Multi-stage build | Correto — imagem final sem SDK |
| Imagem base `aspnet:10.0` | Correto |
| `BUILD_CONFIGURATION=Release` | Correto |
| Usuário não-root | Ausente — container roda como root |
| Health check no container da API | Ausente |
| `.dockerignore` | Não encontrado — pode copiar desnecessários no build |

---

## docker-compose.yml

### Serviços

#### cloudgamecatalog.api

```yaml
image: ${DOCKER_REGISTRY-}cloudgamecatalogapi
build:
  context: .
  dockerfile: ./src/CloudGameCatalog.Api/Dockerfile
ports:
  - '57398:8080'
  - '57399:8081'
restart: unless-stopped
depends_on:
  sqlserver:
    condition: service_healthy
networks:
  - cloudgame-network
```

#### sqlserver

```yaml
image: mcr.microsoft.com/mssql/server:2022-latest
hostname: 'sqlserver'
environment:
  ACCEPT_EULA: 'Y'
  MSSQL_SA_PASSWORD: "CloudGame#2026"
volumes:
  - sql_data:/var/opt/mssql
restart: unless-stopped
ports:
  - '11433:1433'
expose:
  - 1433
healthcheck:
  test: ['CMD-SHELL', '/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$$MSSQL_SA_PASSWORD" -C -Q "SELECT 1" -b -o /dev/null']
  interval: 10s
  timeout: 5s
  retries: 10
  start_period: 30s
networks:
  - cloudgame-network
```

### Volumes e Redes

```yaml
volumes:
  sql_data:         # Volume nomeado para persistência do banco

networks:
  cloudgame-network:
    driver: bridge  # Rede isolada para comunicação entre serviços
```

---

## Mapeamento de Portas

| Serviço | Porta Host | Porta Container | Protocolo |
|---------|-----------|-----------------|-----------|
| API (HTTP) | 57398 | 8080 | HTTP |
| API (HTTPS) | 57399 | 8081 | HTTPS |
| SQL Server | 11433 | 1433 | TCP |

---

## Health Check do SQL Server

```yaml
healthcheck:
  test: sqlcmd -S localhost -U sa -P "$$MSSQL_SA_PASSWORD" -C -Q "SELECT 1"
  interval: 10s
  timeout: 5s
  retries: 10
  start_period: 30s
```

- `start_period: 30s` — aguarda 30s antes de iniciar verificações (SQL Server é lento para iniciar)
- `retries: 10` — até 10 tentativas antes de declarar unhealthy
- `depends_on: condition: service_healthy` — API só sobe após SQL Server estar saudável

---

## docker-compose.override.yml

```yaml
services:
  cloudgamecatalog.api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_HTTP_PORTS=8080
    ports:
      - '8080'  # porta dinâmica no host
```

Override para desenvolvimento local sobrescreve o compose principal.

---

## Pontos de Atenção

| Problema | Detalhe | Severidade |
|---------|---------|-----------|
| Senha SA hardcoded | `"CloudGame#2026"` exposta no yaml | Alta |
| Sem variáveis de ambiente para secrets | Deveria usar `.env` ou Docker secrets | Alta |
| Sem `.dockerignore` | Build pode copiar `obj/`, `bin/`, `.git/` desnecessariamente | Média |
| Container da API roda como root | Risco de segurança em produção | Média |
| `JwtSettings` não injetada via variáveis de ambiente | Ausente no compose — falha em produção | Alta |
| Connection string não injetada via env | `appsettings.json` usa `Server=sqlserver` que depende do hostname do Docker | Média |
| Sem init script para criação do schema | Banco criado sem tabelas — migrations não são executadas automaticamente | Alta |

---

## Fluxo de Inicialização

```
docker-compose up
    │
    ├──► sqlserver: inicia e aguarda 30s
    │         │
    │         └──► healthcheck: SELECT 1 a cada 10s
    │                   │
    │                   └──► service_healthy após ~30-60s
    │
    └──► cloudgamecatalog.api: aguarda sqlserver healthy
              │
              └──► dotnet CloudGameCatalog.Api.dll
                        │
                        └──► ERRO: JwtSettings não encontrado em produção
```

---

## Como Executar

```bash
# Build e subir todos os serviços
docker-compose up --build

# Apenas SQL Server
docker-compose up sqlserver

# Em background
docker-compose up -d

# Parar tudo
docker-compose down

# Parar e remover volumes (apaga o banco)
docker-compose down -v
```

---

## Avaliação da Configuração Docker

| Critério | Avaliação |
|---------|-----------|
| Multi-stage Dockerfile | Correto |
| Health check no SQL Server | Correto |
| `depends_on: service_healthy` | Correto |
| Rede isolada entre serviços | Correto |
| Volume para persistência | Correto |
| Secrets não externalizados | Problema de segurança |
| Schema do banco não criado automaticamente | Bloqueante para primeiro uso |
| `.dockerignore` ausente | Builds mais lentos |
