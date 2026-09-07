using BuildingBlocks.Contracts.Events.Inventory;
using BuildingBlocks.Contracts.Events.Ordering;
using BuildingBlocks.Contracts.Events.Payment;
using BuildingBlocks.Contracts.Events.Shipping;
using MassTransit;
using System.Collections.Concurrent;

namespace MiniCommerce.IntegrationTests.Support;

public sealed class EventProbeConsumer :
    IConsumer<OrderConfirmedIntegrationEvent>,
    IConsumer<InventoryReservedIntegrationEvent>,
    IConsumer<PaymentRequestedIntegrationEvent>,
    IConsumer<PaymentCompletedIntegrationEvent>,
    IConsumer<PaymentFailedIntegrationEvent>,
    IConsumer<ShippingRequestedIntegrationEvent>,
    IConsumer<ShipmentCreatedIntegrationEvent>,
    IConsumer<ReleaseInventoryRequestedIntegrationEvent>,
    IConsumer<InventoryReleasedIntegrationEvent>,
    IConsumer<Fault<PaymentRequestedIntegrationEvent>>,
    IConsumer<Fault<ReserveInventoryRequestedIntegrationEvent>>
{
    private readonly EventProbeStore _store;

    public EventProbeConsumer(EventProbeStore store)
    {
        _store = store;
    }

    public Task Consume(ConsumeContext<OrderConfirmedIntegrationEvent> context) => Record(context.Message);

    public Task Consume(ConsumeContext<InventoryReservedIntegrationEvent> context) => Record(context.Message);

    public Task Consume(ConsumeContext<PaymentRequestedIntegrationEvent> context) => Record(context.Message);

    public Task Consume(ConsumeContext<PaymentCompletedIntegrationEvent> context) => Record(context.Message);

    public Task Consume(ConsumeContext<PaymentFailedIntegrationEvent> context) => Record(context.Message);

    public Task Consume(ConsumeContext<ShippingRequestedIntegrationEvent> context) => Record(context.Message);

    public Task Consume(ConsumeContext<ShipmentCreatedIntegrationEvent> context) => Record(context.Message);

    public Task Consume(ConsumeContext<ReleaseInventoryRequestedIntegrationEvent> context) => Record(context.Message);

    public Task Consume(ConsumeContext<InventoryReleasedIntegrationEvent> context) => Record(context.Message);

    public Task Consume(ConsumeContext<Fault<PaymentRequestedIntegrationEvent>> context) => Record(context.Message);

    public Task Consume(ConsumeContext<Fault<ReserveInventoryRequestedIntegrationEvent>> context) => Record(context.Message);

    private Task Record<T>(T message) where T : class
    {
        _store.Add(message);
        return Task.CompletedTask;
    }
}

public sealed class EventProbeStore
{
    private readonly ConcurrentDictionary<Type, ConcurrentQueue<object>> _messages = new();

    public void Add<T>(T message) where T : class =>
        _messages.GetOrAdd(typeof(T), _ => new ConcurrentQueue<object>()).Enqueue(message);

    public IReadOnlyList<T> Get<T>() where T : class =>
        _messages.TryGetValue(typeof(T), out var messages)
            ? messages.Cast<T>().ToArray()
            : Array.Empty<T>();

    public string Describe() =>
        _messages.Count == 0
            ? "none"
            : string.Join(
                ", ",
                _messages
                    .OrderBy(pair => pair.Key.Name)
                    .Select(pair => $"{pair.Key.Name}={pair.Value.Count}"));
}
