using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events.Ordering;

public sealed record OrderCreatedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }

    public Guid ProductId { get; init; }

    public int Quantity { get; init; }
}
