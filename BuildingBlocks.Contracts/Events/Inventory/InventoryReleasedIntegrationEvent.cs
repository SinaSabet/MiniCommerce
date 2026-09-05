using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events.Inventory;

public sealed record InventoryReleasedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }

    public Guid ReservationId { get; init; }

    public IReadOnlyCollection<Guid> ReservationIds { get; init; }
        = Array.Empty<Guid>();
}
