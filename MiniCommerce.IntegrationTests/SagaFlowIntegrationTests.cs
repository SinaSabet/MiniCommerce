using BuildingBlocks.Contracts.Events.Inventory;
using BuildingBlocks.Contracts.Events.Ordering;
using BuildingBlocks.Contracts.Events.Payment;
using BuildingBlocks.Contracts.Events.Shipping;
using Inventory.Domain.Reservations;
using MassTransit;
using MiniCommerce.IntegrationTests.Fixtures;
using MiniCommerce.IntegrationTests.Support;
using Payment.Domain.PaymentTransactions;
using Xunit;

namespace MiniCommerce.IntegrationTests;

[Collection(IntegrationContainersCollection.Name)]
public sealed class SagaFlowIntegrationTests
{
    private static readonly TimeSpan CompensationTimeout = TimeSpan.FromMinutes(3);

    private readonly IntegrationContainersFixture _containers;

    public SagaFlowIntegrationTests(IntegrationContainersFixture containers)
    {
        _containers = containers;
    }

    [Fact]
    public async Task Multi_item_order_should_complete_the_happy_path()
    {
        await using var system = await MiniCommerceSystem.StartAsync(_containers);
        var (orderId, productIds) = await system.SeedAndConfirmOrderAsync(
            (Stock: 20, Quantity: 3, UnitPrice: 1000m),
            (Stock: 15, Quantity: 2, UnitPrice: 2500m));

        Assert.True(await system.Harness.Consumed.Any<OrderConfirmedIntegrationEvent>());
        Assert.Equal(2, system.Probe.Get<OrderConfirmedIntegrationEvent>().Single().Items.Count);
        Assert.True(await system.Harness.Consumed.Any<InventoryReservedIntegrationEvent>());
        Assert.True(await system.Harness.Consumed.Any<PaymentRequestedIntegrationEvent>());

        var reservations = await system.WaitForReservationsAsync(orderId, ReservationStatus.Reserved, 2);
        var paymentId = await system.WaitForPaymentAsync(orderId);
        await system.CompletePaymentAsync(paymentId);

        await system.WaitForSagaStateAsync(orderId, "AwaitingShipping");
        Assert.True(await system.Harness.Consumed.Any<ShippingRequestedIntegrationEvent>());
        await system.PublishAsync(new ShipmentCreatedIntegrationEvent
        {
            OrderId = orderId,
            ShipmentId = Guid.NewGuid()
        });

        var saga = await system.WaitForSagaStateAsync(orderId, "Completed");
        var payment = await system.GetPaymentAsync(paymentId);
        var inventory = await system.GetInventoryAsync(productIds);

        Assert.True(await system.Harness.Consumed.Any<PaymentCompletedIntegrationEvent>());
        Assert.True(saga.InventoryReserved);
        Assert.True(saga.PaymentCompleted);
        Assert.False(saga.CompensationStarted);
        Assert.Equal(2, saga.ReservationIds.Count);
        Assert.Equal(PaymentStatus.Completed, payment.Status);
        Assert.Equal(2, reservations.Count);
        Assert.Equal((20, 3), inventory[0]);
        Assert.Equal((15, 2), inventory[1]);
    }

    [Fact]
    public async Task Failed_payment_should_release_every_reservation_and_cancel_the_saga()
    {
        await using var system = await MiniCommerceSystem.StartAsync(_containers);
        var (orderId, productIds) = await system.SeedAndConfirmOrderAsync(
            (Stock: 20, Quantity: 3, UnitPrice: 1000m),
            (Stock: 15, Quantity: 2, UnitPrice: 2500m));

        var paymentId = await system.WaitForPaymentAsync(orderId);
        await system.WaitForReservationsAsync(orderId, ReservationStatus.Reserved, 2);
        await system.FailPaymentAsync(paymentId);

        Assert.True(await system.Harness.Consumed.Any<PaymentFailedIntegrationEvent>());
        Assert.True(await system.Harness.Consumed.Any<ReleaseInventoryRequestedIntegrationEvent>());
        Assert.True(await system.Harness.Consumed.Any<InventoryReleasedIntegrationEvent>());

        var released = await system.WaitForReservationsAsync(
            orderId,
            ReservationStatus.Released,
            2,
            CompensationTimeout);
        var saga = await system.WaitForSagaStateAsync(
            orderId,
            "Cancelled",
            CompensationTimeout);
        var inventory = await system.GetInventoryAsync(productIds);
        var payment = await system.GetPaymentAsync(paymentId);

        Assert.Equal(2, released.Count);
        Assert.True(saga.CompensationStarted);
        Assert.True(saga.InventoryReleased);
        Assert.Equal(PaymentStatus.Failed, payment.Status);
        Assert.Equal((20, 0), inventory[0]);
        Assert.Equal((15, 0), inventory[1]);
    }

    [Fact]
    public async Task Duplicate_release_requests_should_not_release_stock_twice()
    {
        await using var system = await MiniCommerceSystem.StartAsync(_containers);
        var (orderId, productIds) = await system.SeedAndConfirmOrderAsync(
            (Stock: 12, Quantity: 4, UnitPrice: 1000m),
            (Stock: 9, Quantity: 3, UnitPrice: 2000m));

        var reservations = await system.WaitForReservationsAsync(orderId, ReservationStatus.Reserved, 2);
        var reservationIds = reservations.Select(x => x.Id).ToArray();
        var duplicate = new ReleaseInventoryRequestedIntegrationEvent
        {
            OrderId = orderId,
            ReservationId = reservationIds[0],
            ReservationIds = reservationIds
        };

        await system.PublishAsync(duplicate);
        await system.PublishAsync(duplicate);
        await system.PublishAsync(duplicate);

        await system.WaitForReservationsAsync(
            orderId,
            ReservationStatus.Released,
            2,
            CompensationTimeout);
        var inventory = await system.GetInventoryAsync(productIds);

        Assert.Equal((12, 0), inventory[0]);
        Assert.Equal((9, 0), inventory[1]);
    }

    [Fact]
    public async Task Ordering_outbox_should_deliver_after_the_broker_recovers()
    {
        await using var system = await MiniCommerceSystem.StartAsync(_containers);

        await _containers.RabbitMq.PauseAsync();
        try
        {
            var (orderId, _) = await system.SeedAndConfirmOrderAsync(
                (Stock: 10, Quantity: 2, UnitPrice: 1000m));

            await system.WaitForOrderingOutboxCountAsync(count => count > 0);

            await _containers.RabbitMq.UnpauseAsync();
            await _containers.WaitUntilRabbitMqIsReadyAsync();

            await system.WaitForSagaStateAsync(
                orderId,
                "AwaitingPayment",
                TimeSpan.FromSeconds(120));
            Assert.True(await system.Harness.Consumed.Any<OrderConfirmedIntegrationEvent>());
            await system.WaitForOrderingOutboxCountAsync(
                count => count == 0,
                TimeSpan.FromSeconds(120));
        }
        finally
        {
            if (_containers.RabbitMq.State == DotNet.Testcontainers.Containers.TestcontainersStates.Paused)
                await _containers.RabbitMq.UnpauseAsync();

            await _containers.WaitUntilRabbitMqIsReadyAsync();
        }
    }

    [Fact]
    public async Task Payment_consumer_fault_should_start_compensation_and_cancel_the_saga()
    {
        await using var system = await MiniCommerceSystem.StartAsync(
            _containers,
            startPayment: false);
        var (orderId, productIds) = await system.SeedAndConfirmOrderAsync(
            (Stock: 10, Quantity: 2, UnitPrice: 1000m));

        await system.WaitForSagaStateAsync(orderId, "AwaitingPayment");
        await system.PublishFaultAsync(
            new PaymentRequestedIntegrationEvent(orderId, 2000m, "IRR"));

        var saga = await system.WaitForSagaStateAsync(
            orderId,
            "Cancelled",
            CompensationTimeout);
        var reservations = await system.WaitForReservationsAsync(
            orderId,
            ReservationStatus.Released,
            1,
            CompensationTimeout);
        var inventory = await system.GetInventoryAsync(productIds);

        Assert.True(await system.Harness.Consumed.Any<Fault<PaymentRequestedIntegrationEvent>>());
        Assert.True(await system.Harness.Consumed.Any<ReleaseInventoryRequestedIntegrationEvent>());
        Assert.True(saga.CompensationStarted);
        Assert.True(saga.InventoryReleased);
        Assert.NotNull(saga.FailedAt);
        Assert.Single(reservations);
        Assert.Equal((10, 0), inventory[0]);
    }

    [Fact]
    public async Task Inventory_consumer_fault_should_fail_the_saga_without_compensation()
    {
        await using var system = await MiniCommerceSystem.StartAsync(
            _containers,
            startInventory: false);
        var (orderId, _) = await system.ConfirmOrderWithoutInventoryAsync(
            (Stock: 0, Quantity: 1, UnitPrice: 1000m));

        await system.WaitForSagaStateAsync(orderId, "AwaitingInventory");
        await system.PublishFaultAsync(
            new ReserveInventoryRequestedIntegrationEvent
            {
                OrderId = orderId,
                Items = Array.Empty<ReserveInventoryItem>()
            });

        var saga = await system.WaitForSagaStateAsync(orderId, "Failed");

        Assert.True(await system.Harness.Consumed.Any<Fault<ReserveInventoryRequestedIntegrationEvent>>());
        Assert.NotNull(saga.FailedAt);
        Assert.False(saga.CompensationStarted);
        Assert.False(saga.InventoryReserved);
        Assert.Empty(saga.ReservationIds);
    }
}
