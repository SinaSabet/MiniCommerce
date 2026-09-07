FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY MiniCommerce.sln .
COPY BuildingBlocks ./BuildingBlocks
COPY BuildingBlocks.Contracts ./BuildingBlocks.Contracts
COPY Ordering.Application ./Ordering.Application
COPY Ordering.Domain ./Ordering.Domain
COPY Ordering.Infrastructure ./Ordering.Infrastructure
COPY Inventory.Application ./Inventory.Application
COPY Inventory.Domain ./Inventory.Domain
COPY Inventory.Infrastructure ./Inventory.Infrastructure
COPY Payment.Application ./Payment.Application
COPY Payment.Domain ./Payment.Domain
COPY Payment.Infrastructure ./Payment.Infrastructure
COPY Shipping.Application ./Shipping.Application
COPY Shipping.Domain ./Shipping.Domain
COPY Shipping.Infrastructure ./Shipping.Infrastructure
COPY MiniCommerce.DatabaseMigrator ./MiniCommerce.DatabaseMigrator

RUN dotnet restore MiniCommerce.DatabaseMigrator/MiniCommerce.DatabaseMigrator.csproj
RUN dotnet publish MiniCommerce.DatabaseMigrator/MiniCommerce.DatabaseMigrator.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime

WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MiniCommerce.DatabaseMigrator.dll"]
