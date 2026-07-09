# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine AS base
RUN apk add --no-cache icu-libs

# Informa ao .NET para desativar o modo invariante e usar o ICU
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

USER $APP_UID
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/CloudGameCatalog.Consumer/CloudGameCatalog.Consumer.csproj", "src/CloudGameCatalog.Consumer/"]
COPY ["src/CloudGameCatalog.Application/CloudGameCatalog.Application.csproj", "src/CloudGameCatalog.Application/"]
COPY ["src/CloudGameCatalog.Domain/CloudGameCatalog.Domain.csproj", "src/CloudGameCatalog.Domain/"]
COPY ["src/CloudGameCatalog.Infrastructure/CloudGameCatalog.Infrastructure.csproj", "src/CloudGameCatalog.Infrastructure/"]
RUN dotnet restore "./src/CloudGameCatalog.Consumer/CloudGameCatalog.Consumer.csproj"
COPY . .
WORKDIR "/src/src/CloudGameCatalog.Consumer"
RUN dotnet build "./CloudGameCatalog.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CloudGameCatalog.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CloudGameCatalog.Consumer.dll"]