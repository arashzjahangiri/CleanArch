FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

# Specifies whether .NET welcome and telemetry messages are displayed on the first run.
ENV DOTNET_NOLOGO=1

# Specifies whether data about the .NET tools usage is collected and sent to Microsoft.
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1

# The base image ships a non-root "app" user; run as it rather than root.
USER app

FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim AS build
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["nuget.config", "."]
COPY ["src/Shop.PublicApi/Shop.PublicApi.csproj", "src/Shop.PublicApi/"]
COPY ["src/Shop.Application/Shop.Application.csproj", "src/Shop.Application/"]
COPY ["src/Shop.Domain/Shop.Domain.csproj", "src/Shop.Domain/"]
COPY ["src/Shop.Core/Shop.Core.csproj", "src/Shop.Core/"]
COPY ["src/Shop.Infrastructure/Shop.Infrastructure.csproj", "src/Shop.Infrastructure/"]
COPY ["src/Shop.Query/Shop.Query.csproj", "src/Shop.Query/"]
RUN dotnet restore "src/Shop.PublicApi/Shop.PublicApi.csproj"
COPY . .

# Publish straight from the restored sources; a separate build stage only duplicated this work.
FROM build AS publish
RUN dotnet publish "src/Shop.PublicApi/Shop.PublicApi.csproj" \
    --configuration Release --no-restore --nologo --output /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shop.PublicApi.dll"]
