FROM docker-registry-002.zeuslearning.com/zeuslearning/dotnet/sdk:10.0-alpine AS base
WORKDIR /app

FROM docker-registry-002.zeuslearning.com/zeuslearning/dotnet/sdk:10.0-alpine AS build
ARG CODEARTIFACT_TOKEN
WORKDIR /src

# Copies BOTH projects so the local assembly references resolve cleanly
COPY ["TraineeManagement.Worker/TraineeManagement.Worker.csproj", "TraineeManagement.Worker/"]
COPY ["TraineeManagement.Api/TraineeManagement.Api.csproj", "TraineeManagement.Api/"]
COPY ["nuget.config", "./"]

# Removed the broken ENV assignment so the ARG value injects straight into this command natively
RUN dotnet nuget update source "zeuslearning/training-nuget-store" --username aws --password "${CODEARTIFACT_TOKEN}" --store-password-in-clear-text --configfile nuget.config

# FIXED: Target the worker project file to start the main assembly restoration process
RUN dotnet restore "TraineeManagement.Worker/TraineeManagement.Worker.csproj" --configfile nuget.config

COPY . .
WORKDIR "/src/TraineeManagement.Worker"
RUN dotnet build "TraineeManagement.Worker.csproj" -c Release -o /app/build

FROM build AS publish
# FIXED: Appended the duplicate output files merge property flag right here
RUN dotnet publish "TraineeManagement.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false /p:ErrorOnDuplicatePublishOutputFiles=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TraineeManagement.Worker.dll"]
