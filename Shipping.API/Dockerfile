# ===============================
# Build Stage
# ===============================

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY MiniCommerce.sln .
COPY Shipping.API ./Shipping.API
COPY Shipping.Application ./Shipping.Application
COPY Shipping.Domain ./Shipping.Domain
COPY Shipping.Infrastructure ./Shipping.Infrastructure

RUN dotnet restore Shipping.API/Shipping.API.csproj

RUN dotnet publish \
    Shipping.API/Shipping.API.csproj \
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

ENTRYPOINT ["dotnet", "Shipping.API.dll"]
