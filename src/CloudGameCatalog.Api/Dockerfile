FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base

# Instala os pacotes ICU para habilitar suporte a dados e strings globais
RUN apk add --no-cache icu-libs

# Desativa o modo invariante de globalização exigido pelo SQL Server
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Define o diretório de trabalho antes de configurar o usuário
WORKDIR /app
EXPOSE 8080

# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/CloudGameCatalog.Api/CloudGameCatalog.Api.csproj", "src/CloudGameCatalog.Api/"]
COPY ["src/CloudGameCatalog.Application/CloudGameCatalog.Application.csproj", "src/CloudGameCatalog.Application/"]
COPY ["src/CloudGameCatalog.Domain/CloudGameCatalog.Domain.csproj", "src/CloudGameCatalog.Domain/"]
COPY ["src/CloudGameCatalog.Infrastructure/CloudGameCatalog.Infrastructure.csproj", "src/CloudGameCatalog.Infrastructure/"]

RUN dotnet restore "./src/CloudGameCatalog.Api/CloudGameCatalog.Api.csproj"
COPY . .
WORKDIR "/src/src/CloudGameCatalog.Api"
RUN dotnet build "./CloudGameCatalog.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CloudGameCatalog.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Garante que a pasta de Logs exista e pertença ao usuário não-root do .NET
USER root
RUN mkdir -p /app/Logs && chown -R $APP_UID:$APP_UID /app/Logs

# Altera definitivamente para o usuário seguro do contêiner
USER $APP_UID

ENTRYPOINT ["dotnet", "CloudGameCatalog.Api.dll"]