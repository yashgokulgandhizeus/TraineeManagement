FROM docker-registry-002.zeuslearning.com/zeuslearning/dotnet/sdk:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM docker-registry-002.zeuslearning.com/zeuslearning/dotnet/sdk:10.0-alpine AS build
ARG CODEARTIFACT_TOKEN
WORKDIR /src

COPY ["TrainingDirectory.Api/TrainingDirectory.Api.csproj", "TrainingDirectory.Api/"]
COPY ["nuget.config", "./"]

# Removed the broken ENV line so the token injects directly
RUN dotnet nuget update source "zeuslearning/training-nuget-store" --username aws --password "${CODEARTIFACT_TOKEN}" --store-password-in-clear-text --configfile nuget.config

# FIXED: Restores its own project directory path cleanly
RUN dotnet restore "TrainingDirectory.Api/TrainingDirectory.Api.csproj" --configfile nuget.config

COPY . .
WORKDIR "/src/TrainingDirectory.Api"
RUN dotnet build "TrainingDirectory.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrainingDirectory.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TrainingDirectory.Api.dll"]
