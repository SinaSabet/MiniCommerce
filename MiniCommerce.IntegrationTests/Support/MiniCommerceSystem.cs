using Inventory.Application.Inventory.Commands.AddStock;
using Inventory.Domain.Reservations;
using Inventory.Infrastructure.Persistence;
using MassTransit;
using MassTransit.Testing;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MiniCommerce.IntegrationTests.Fixtures;
using Ordering.Application.Orders.Commands.ConfirmOrder;
using Ordering.Domain.Orders;
using Ordering.Domain.ValueObjects;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Saga;
using Payment.Application.Payments.Commands.CompletePayment;
using Payment.Application.Payments.Commands.FailPayment;
using Payment.Domain.PaymentTransactions;
using Payment.Infrastructure.Persistence;

namespace MiniCommerce.IntegrationTests.Support;

public sealed class MiniCommerceSystem : IAsyncDisposable
{
    private readonly IntegrationContainersFixture _containers;
    private ServiceProvider? _probeProvider;
    private IHost? _orderingHost;
    private IHost? _inventoryHost;
    private IHost? _paymentHost;

    private MiniCommerceSystem(
        IntegrationContainersFixture containers,
        string orderingConnection,
        string inventoryConnection,
        string paymentConnection,
        string virtualHost)
    {
        _containers = containers;
        OrderingConnection = orderingConnection;
        InventoryConnection = inventoryConnection;
        PaymentConnection = paymentConnection;
        VirtualHost = virtualHost;
    }

    public string OrderingConnection { get; }
    public string InventoryConnection { get; }
    public string PaymentConnection { get; }
    public string VirtualHost { get; }
    public ITestHarness Harness { get; private set; } = default!;
    public EventProbeStore Probe { get; private set; } = default!;

    public static async Task<MiniCommerceSystem> StartAsync(
        IntegrationContainersFixture containers,
        bool startInventory = true,
        bool startPayment = true,
        CancellationToken cancellationToken = default)
    {
        var suffix = Guid.NewGuid().ToString("N");
        var system = new MiniCommerceSystem(
            containers,
            WithDatabase(containers.SqlServer.GetConnectionString(), $"Ordering_{suffix}"),
            WithDatabase(containers.SqlServer.GetConnectionString(), $"Inventory_{suffix}"),
            WithDatabase(containers.SqlServer.GetConnectionString(), $"Payment_{suffix}"),
            $"test_{suffix}");

        await containers.CreateVirtualHostAsync(system.VirtualHost, cancellationToken);
        await system.MigrateAsync(cancellationToken);
        await system.StartProbeAsync(cancellationToken);

        system._orderingHost = system.CreateOrderingHost();
        if (startInventory)
            system._inventoryHost = system.CreateInventoryHost();
        if (startPayment)
            system._paymentHost = system.CreatePaymentHost();

        await system._orderingHost.StartAsync(cancellationToken);
        if (system._inventoryHost is not null)
            await system._inventoryHost.StartAsync(cancellationToken);
        if (system._paymentHost is not null)
            await system._paymentHost.StartAsync(cancellationToken);

        return system;
    }

    public async Task<(Guid OrderId, Guid[] ProductIds)> SeedAndConfirmOrderAsync(
        params (int Stock, int Quantity, decimal UnitPrice)[] lines) =>
        await CreateAndConfirmOrderAsync(true, lines);

    public async Task<(Guid OrderId, Guid[] ProductIds)> ConfirmOrderWithoutInventoryAsync(
        params (int Stock, int Quantity, decimal UnitPrice)[] lines) =>
        await CreateAndConfirmOrderAsync(false, lines);

    private async Task<(Guid OrderId, Guid[] ProductIds)> CreateAndConfirmOrderAsync(
        bool seedInventory,
        params (int Stock, int Quantity, decimal UnitPrice)[] lines)
    {
        var productIds = lines.Select(_ => Guid.NewGuid()).ToArray();

        if (seedInventory)
        {
            await using var scope = _inventoryHost!.Services.CreateAsyncScope();
            var sender = scope.ServiceProvider.GetRequiredService<ISender>();
            for (var index = 0; index < lines.Length; index++)
                await sender.Send(new AddStockCommand(productIds[index], lines[index].Stock));
        }

        var order = Order.Create(new Address("Tehran", "Integration Test Street", "1234567890"));
        for (var index = 0; index < lines.Length; index++)
        {
            order.AddItem(
                productIds[index],
                $"Product {index + 1}",
                new Money(lines[index].UnitPrice, "IRR"),
                lines[index].Quantity);
        }

        await using (var scope = _orderingHost!.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();
        }

        await using (var scope = _orderingHost.Services.CreateAsyncScope())
        {
            await scope.ServiceProvider.GetRequiredService<ISender>()
                .Send(new ConfirmOrderCommand(order.Id));
        }

        return (order.Id, productIds);
    }

    public async Task<Guid> WaitForPaymentAsync(Guid orderId) =>
        await EventuallyValueAsync(async () =>
        {
            await using var scope = _paymentHost!.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
            return await dbContext.PaymentTransactions
                .AsNoTracking()
                .Where(x => x.OrderId == orderId)
                .Select(x => (Guid?)x.Id)
                .SingleOrDefaultAsync();
        });

    public async Task CompletePaymentAsync(Guid paymentId)
    {
        await using var scope = _paymentHost!.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<ISender>()
            .Send(new CompletePaymentCommand(paymentId));
    }

    public async Task FailPaymentAsync(Guid paymentId)
    {
        await using var scope = _paymentHost!.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<ISender>()
            .Send(new FailPaymentCommand(paymentId, "Declined by integration test"));
    }

    public Task PublishAsync<T>(T message)
        where T : class =>
        _orderingHost!.Services.GetRequiredService<IBus>().Publish(message);

    public Task PublishFaultAsync<T>(T message)
        where T : class =>
        _orderingHost!.Services.GetRequiredService<IBus>().Publish<Fault<T>>(
            new
            {
                FaultId = NewId.NextGuid(),
                FaultedMessageId = NewId.NextGuid(),
                Timestamp = DateTime.UtcNow,
                Exceptions = Array.Empty<ExceptionInfo>(),
                Host = new
                {
                    MachineName = "integration-tests",
                    ProcessName = "dotnet-test",
                    ProcessId = Environment.ProcessId,
                    Assembly = typeof(MiniCommerceSystem).Assembly.GetName().Name,
                    AssemblyVersion = typeof(MiniCommerceSystem).Assembly.GetName().Version?.ToString(),
                    FrameworkVersion = Environment.Version.ToString(),
                    MassTransitVersion = typeof(IBus).Assembly.GetName().Version?.ToString(),
                    OperatingSystemVersion = Environment.OSVersion.VersionString
                },
                FaultMessageTypes = new[] { MessageUrn.ForType<T>().ToString() },
                Message = message
            });

    public async Task<OrderSagaState> WaitForSagaStateAsync(
        Guid orderId,
        string state,
        TimeSpan? timeout = null)
    {
        var wait = timeout ?? TimeSpan.FromSeconds(45);
        var deadline = DateTime.UtcNow + wait;
        string? lastObservedState = null;

        while (DateTime.UtcNow < deadline)
        {
            await using var scope = _orderingHost!.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderSagaDbContext>();
            var saga = await dbContext.Set<OrderSagaState>()
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.OrderId == orderId);

            lastObservedState = saga?.CurrentState;
            if (lastObservedState == state)
                return saga!;

            await Task.Delay(200);
        }

        throw new TimeoutException(
            $"Saga {orderId} did not reach state '{state}' within {wait}. " +
            $"Last observed state: '{lastObservedState ?? "not-created"}'. " +
            $"Observed events: {Probe.Describe()}.");
    }

    public Task<IReadOnlyList<InventoryReservation>> WaitForReservationsAsync(
        Guid orderId,
        ReservationStatus status,
        int expectedCount,
        TimeSpan? timeout = null) =>
        EventuallyAsync<IReadOnlyList<InventoryReservation>>(async () =>
        {
            await using var scope = _inventoryHost!.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
            var reservations = await dbContext.InventoryReservations
                .AsNoTracking()
                .Where(x => x.OrderId == orderId && x.Status == status)
                .ToListAsync();
            return reservations.Count == expectedCount ? reservations : null;
        }, timeout);

    public async Task<(int OnHand, int Reserved)[]> GetInventoryAsync(Guid[] productIds)
    {
        await using var scope = _inventoryHost!.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
        var items = await dbContext.InventoryItems
            .AsNoTracking()
            .Where(x => productIds.Contains(x.ProductId))
            .ToDictionaryAsync(x => x.ProductId);

        return productIds
            .Select(id => (items[id].OnHandQuantity, items[id].ReservedQuantity))
            .ToArray();
    }

    public async Task<PaymentTransaction> GetPaymentAsync(Guid paymentId)
    {
        await using var scope = _paymentHost!.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
        return await dbContext.PaymentTransactions.AsNoTracking().SingleAsync(x => x.Id == paymentId);
    }

    public async Task<int> CountOrderingOutboxMessagesAsync()
    {
        await using var scope = _orderingHost!.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
        return await dbContext.Set<MassTransit.EntityFrameworkCoreIntegration.OutboxMessage>().CountAsync();
    }

    public async Task WaitForOrderingOutboxCountAsync(
        Func<int, bool> predicate,
        TimeSpan? timeout = null)
    {
        var deadline = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(45));
        while (DateTime.UtcNow < deadline)
        {
            if (predicate(await CountOrderingOutboxMessagesAsync()))
                return;
            await Task.Delay(200);
        }
        throw new TimeoutException("Ordering outbox did not reach the expected state.");
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var host in new[] { _paymentHost, _inventoryHost, _orderingHost })
        {
            if (host is null)
                continue;
            await host.StopAsync();
            host.Dispose();
        }

        if (_probeProvider is not null)
            await _probeProvider.DisposeAsync();
    }

    private async Task StartProbeAsync(CancellationToken cancellationToken)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));
        services.AddSingleton<EventProbeStore>();
        services.AddMassTransitTestHarness(x =>
        {
            x.SetTestTimeouts(TimeSpan.FromSeconds(90), TimeSpan.FromSeconds(10));
            x.AddConsumer<EventProbeConsumer>();
            x.UsingRabbitMq((context, cfg) =>
            {
                ConfigureRabbitMq(cfg);
                cfg.ConfigureEndpoints(context);
            });
        });

        _probeProvider = services.BuildServiceProvider(true);
        Probe = _probeProvider.GetRequiredService<EventProbeStore>();
        Harness = await _probeProvider.StartTestHarness();
    }

    private IHost CreateOrderingHost() => CreateHost((services, configuration) =>
    {
        Ordering.Application.DependencyInjection.AddApplication(services);
        Ordering.Infrastructure.DependencyInjection.AddInfrastructure(services, configuration);
    });

    private IHost CreateInventoryHost() => CreateHost((services, configuration) =>
    {
        Inventory.Application.DependencyInjection.AddApplication(services);
        Inventory.Infrastructure.DependencyInjection.AddInfrastructure(services, configuration);
    });

    private IHost CreatePaymentHost() => CreateHost((services, configuration) =>
    {
        Payment.Application.DependencyInjection.AddApplication(services);
        Payment.Infrastructure.DependencyInjection.AddInfrastructure(services, configuration);
    });

    private IHost CreateHost(Action<IServiceCollection, IConfiguration> configureServices)
    {
        var settings = new Dictionary<string, string?>
        {
            ["ConnectionStrings:OrderingConnection"] = OrderingConnection,
            ["ConnectionStrings:InventoryConnection"] = InventoryConnection,
            ["ConnectionStrings:PaymentConnection"] = PaymentConnection,
            ["RabbitMQ:Host"] = _containers.RabbitMq.Hostname,
            ["RabbitMQ:Port"] = _containers.RabbitMq.GetMappedPublicPort(5672).ToString(),
            ["RabbitMQ:VirtualHost"] = VirtualHost,
            ["RabbitMQ:Username"] = IntegrationContainersFixture.RabbitUsername,
            ["RabbitMQ:Password"] = IntegrationContainersFixture.RabbitPassword
        };

        return Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(configuration => configuration.AddInMemoryCollection(settings))
            .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning))
            .ConfigureServices((context, services) =>
            {
                configureServices(services, context.Configuration);
                services.Configure<MassTransitHostOptions>(options =>
                {
                    options.WaitUntilStarted = true;
                    options.StartTimeout = TimeSpan.FromMinutes(2);
                    options.StopTimeout = TimeSpan.FromMinutes(1);
                });
            })
            .Build();
    }

    private void ConfigureRabbitMq(IRabbitMqBusFactoryConfigurator cfg)
    {
        cfg.Host(
            _containers.RabbitMq.Hostname,
            _containers.RabbitMq.GetMappedPublicPort(5672),
            VirtualHost,
            host =>
            {
                host.Username(IntegrationContainersFixture.RabbitUsername);
                host.Password(IntegrationContainersFixture.RabbitPassword);
            });
    }

    private async Task MigrateAsync(CancellationToken cancellationToken)
    {
        await using var ordering = new OrderingDbContext(
            new DbContextOptionsBuilder<OrderingDbContext>().UseSqlServer(OrderingConnection).Options);
        await ordering.Database.MigrateAsync(cancellationToken);

        await using var saga = new OrderSagaDbContext(
            new DbContextOptionsBuilder<OrderSagaDbContext>().UseSqlServer(OrderingConnection).Options);
        await saga.Database.MigrateAsync(cancellationToken);

        await using var inventory = new InventoryDbContext(
            new DbContextOptionsBuilder<InventoryDbContext>().UseSqlServer(InventoryConnection).Options);
        await inventory.Database.MigrateAsync(cancellationToken);

        await using var payment = new PaymentDbContext(
            new DbContextOptionsBuilder<PaymentDbContext>().UseSqlServer(PaymentConnection).Options);
        await payment.Database.MigrateAsync(cancellationToken);
    }

    private static string WithDatabase(string connectionString, string database)
    {
        var builder = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = database };
        return builder.ConnectionString;
    }

    private static async Task<T> EventuallyAsync<T>(
        Func<Task<T?>> probe,
        TimeSpan? timeout = null)
        where T : class
    {
        var wait = timeout ?? TimeSpan.FromSeconds(45);
        var deadline = DateTime.UtcNow + wait;
        while (DateTime.UtcNow < deadline)
        {
            var value = await probe();
            if (value is not null)
                return value;
            await Task.Delay(200);
        }
        throw new TimeoutException($"Expected state was not reached within {wait}.");
    }

    private static async Task<Guid> EventuallyValueAsync(
        Func<Task<Guid?>> probe,
        TimeSpan? timeout = null)
    {
        var wait = timeout ?? TimeSpan.FromSeconds(45);
        var deadline = DateTime.UtcNow + wait;
        while (DateTime.UtcNow < deadline)
        {
            var value = await probe();
            if (value.HasValue)
                return value.Value;
            await Task.Delay(200);
        }
        throw new TimeoutException($"Expected value was not produced within {wait}.");
    }
}
