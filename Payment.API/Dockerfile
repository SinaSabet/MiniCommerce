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


# Copy Payment Microservice

COPY Payment.API ./Payment.API
COPY Payment.Application ./Payment.Application
COPY Payment.Domain ./Payment.Domain
COPY Payment.Infrastructure ./Payment.Infrastructure



# Restore

RUN dotnet restore Payment.API/Payment.API.csproj



# Publish

RUN dotnet publish \
    Payment.API/Payment.API.csproj \
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


ENTRYPOINT ["dotnet","Payment.API.dll"]
