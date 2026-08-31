using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events.Ordering;

public sealed record OrderConfirmedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }

    public List<OrderItemMessage> Items { get; init; } = new();
}

public sealed record OrderItemMessage
{
    public Guid ProductId { get; init; }

    public int Quantity { get; init; }
}
