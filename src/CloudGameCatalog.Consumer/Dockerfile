FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base

# Instala os pacotes ICU para habilitar suporte a dados e strings globais
RUN apk add --no-cache icu-libs

# Desativa o modo invariante de globalização exigido pelo SQL Server
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Define o diretório de trabalho antes de configurar o usuário
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/CloudGameCatalog.Consumer/CloudGameCatalog.Consumer.csproj", "src/CloudGameCatalog.Consumer/"]
RUN dotnet restore "./src/CloudGameCatalog.Consumer/CloudGameCatalog.Consumer.csproj"
COPY . .
WORKDIR "/src/src/CloudGameCatalog.Consumer"
RUN dotnet build "./CloudGameCatalog.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CloudGameCatalog.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Garante que a pasta de Logs exista e pertença ao usuário não-root do .NET (evita erro do Serilog)
USER root
RUN mkdir -p /app/Logs && chown -R $APP_UID:$APP_UID /app/Logs

# Altera definitivamente para o usuário seguro do contêiner
USER $APP_UID

ENTRYPOINT ["dotnet", "CloudGameCatalog.Consumer.dll"]