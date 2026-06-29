FROM docker-registry-002.zeuslearning.com/zeuslearning/dotnet/sdk:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM docker-registry-002.zeuslearning.com/zeuslearning/dotnet/sdk:10.0-alpine AS build
ARG CODEARTIFACT_TOKEN
WORKDIR /src

COPY ["TraineeManagement.Api/TraineeManagement.Api.csproj", "TraineeManagement.Api/"]
COPY ["nuget.config", "./"]

# Removed the broken ENV line so the ARG values inject straight into this command natively
RUN dotnet nuget update source "zeuslearning/training-nuget-store" --username aws --password "${CODEARTIFACT_TOKEN}" --store-password-in-clear-text --configfile nuget.config

RUN dotnet restore "TraineeManagement.Api/TraineeManagement.Api.csproj" --configfile nuget.config

COPY . .
WORKDIR "/src/TraineeManagement.Api"
RUN dotnet build "TraineeManagement.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TraineeManagement.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TraineeManagement.Api.dll"]
