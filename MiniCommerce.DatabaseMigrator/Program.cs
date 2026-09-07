using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Saga;
using Payment.Infrastructure.Persistence;
using Shipping.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);

var connections = new
{
    Ordering = GetConnectionString(builder.Configuration, "OrderingConnection"),
    OrderSaga = GetConnectionString(
        builder.Configuration,
        "OrderSagaConnection",
        "OrderingConnection"),
    Inventory = GetConnectionString(builder.Configuration, "InventoryConnection"),
    Payment = GetConnectionString(builder.Configuration, "PaymentConnection"),
    Shipping = GetConnectionString(builder.Configuration, "ShippingConnection")
};

await MigrateAsync<OrderingDbContext>(connections.Ordering);
await MigrateAsync<OrderSagaDbContext>(connections.Ordering);
await MigrateAsync<OrderSagaDbContext>(connections.OrderSaga);
await MigrateAsync<InventoryDbContext>(connections.Inventory);
await MigrateAsync<PaymentDbContext>(connections.Payment);
await MigrateAsync<ShippingDbContext>(connections.Shipping);

return;

static string GetConnectionString(
    IConfiguration configuration,
    string name,
    string? fallbackName = null)
{
    var connectionString = configuration.GetConnectionString(name);

    if (string.IsNullOrWhiteSpace(connectionString) && fallbackName is not null)
    {
        connectionString = configuration.GetConnectionString(fallbackName);
    }

    return !string.IsNullOrWhiteSpace(connectionString)
        ? connectionString
        : throw new InvalidOperationException(
            $"Connection string '{name}' was not configured.");
}

static async Task MigrateAsync<TContext>(string connectionString)
    where TContext : DbContext
{
    var options = new DbContextOptionsBuilder<TContext>()
        .UseSqlServer(connectionString)
        .Options;

    await using var context = (TContext)Activator.CreateInstance(
        typeof(TContext),
        options)!;

    await context.Database.MigrateAsync();
}
