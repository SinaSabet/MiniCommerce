using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events.Payment;

public sealed record PaymentCompletedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }

    public Guid PaymentId { get; init; }

    public decimal Amount { get; init; }
}
