# ==========================
# Build Stage
# ==========================

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src


# Copy solution
COPY MiniCommerce.sln .


# Copy Shared
COPY BuildingBlocks ./BuildingBlocks
COPY BuildingBlocks.Contracts ./BuildingBlocks.Contracts


# Copy Ordering Microservice

COPY Ordering.API ./Ordering.API
COPY Ordering.Application ./Ordering.Application
COPY Ordering.Domain ./Ordering.Domain
COPY Ordering.Infrastructure ./Ordering.Infrastructure



# Restore dependencies

RUN dotnet restore Ordering.API/Ordering.API.csproj



# Publish application

RUN dotnet publish \
    Ordering.API/Ordering.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore



# ==========================
# Runtime Stage
# ==========================

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime


WORKDIR /app


COPY --from=build /app/publish .


EXPOSE 8080


ENTRYPOINT ["dotnet", "Ordering.API.dll"]