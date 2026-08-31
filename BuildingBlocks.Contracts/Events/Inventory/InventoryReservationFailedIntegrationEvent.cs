using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events.Inventory;

public sealed record InventoryReservationFailedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }

    public DateTime ReservedAtUtc { get; init; }

    public string Reason { get; init; } = string.Empty;
}
