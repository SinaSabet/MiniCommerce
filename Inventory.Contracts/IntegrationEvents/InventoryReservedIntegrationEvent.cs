namespace Inventory.Contracts.IntegrationEvents;

public sealed record InventoryReservedIntegrationEvent
{
    public Guid OrderId { get; init; }

    public DateTime ReservedAtUtc { get; init; }
}