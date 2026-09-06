using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events.Shipping;

public sealed record ShipmentCreatedIntegrationEvent
    : IntegrationEvent
{
    public Guid OrderId { get; init; }

    public Guid ShipmentId { get; init; }
}
