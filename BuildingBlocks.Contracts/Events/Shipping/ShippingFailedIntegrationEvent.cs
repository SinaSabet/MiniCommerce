using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events.Shipping;

public sealed record ShippingFailedIntegrationEvent
    : IntegrationEvent
{
    public Guid OrderId { get; init; }

    public string Reason { get; init; } = default!;
}
