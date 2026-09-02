using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events.Payment;

public sealed record PaymentFailedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }

    public Guid PaymentId { get; init; }

    public string Reason { get; init; }
}
