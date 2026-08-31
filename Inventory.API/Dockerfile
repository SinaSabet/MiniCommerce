# ===============================
# Build Stage
# ===============================

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src


# Copy solution
COPY MiniCommerce.sln .


# Copy shared libraries
COPY BuildingBlocks ./BuildingBlocks
COPY BuildingBlocks.Contracts ./BuildingBlocks.Contracts


# Copy Inventory Microservice

COPY Inventory.API ./Inventory.API
COPY Inventory.Application ./Inventory.Application
COPY Inventory.Domain ./Inventory.Domain
COPY Inventory.Infrastructure ./Inventory.Infrastructure



# Restore

RUN dotnet restore Inventory.API/Inventory.API.csproj



# Publish

RUN dotnet publish \
    Inventory.API/Inventory.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore



# ===============================
# Runtime Stage
# ===============================

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime


WORKDIR /app


COPY --from=build /app/publish .


EXPOSE 8080


ENTRYPOINT ["dotnet","Inventory.API.dll"]