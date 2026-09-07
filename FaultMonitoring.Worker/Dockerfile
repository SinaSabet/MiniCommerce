# ===============================
# Build Stage
# ===============================

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY BuildingBlocks ./BuildingBlocks
COPY BuildingBlocks.Contracts ./BuildingBlocks.Contracts
COPY FaultMonitoring.Worker ./FaultMonitoring.Worker

RUN dotnet restore FaultMonitoring.Worker/FaultMonitoring.Worker.csproj

RUN dotnet publish \
    FaultMonitoring.Worker/FaultMonitoring.Worker.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ===============================
# Runtime Stage
# ===============================

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FaultMonitoring.Worker.dll"]
