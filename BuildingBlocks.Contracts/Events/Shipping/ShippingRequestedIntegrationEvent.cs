using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events.Shipping;

public sealed record ShippingRequestedIntegrationEvent
    : IntegrationEvent
{
    public Guid OrderId { get; init; }

    public Guid PaymentId { get; init; }
}
