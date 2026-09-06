using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events.Payment;

public sealed record PaymentRefundedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }

    public Guid PaymentId { get; init; }
}
