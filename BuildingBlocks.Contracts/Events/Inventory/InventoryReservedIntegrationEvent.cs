using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events.Inventory;

public sealed record InventoryReservedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }

    public DateTime ReservedAtUtc { get; init; }
}
