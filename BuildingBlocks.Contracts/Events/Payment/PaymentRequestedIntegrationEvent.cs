using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events.Payment;

public sealed record PaymentRequestedIntegrationEvent
    : IntegrationEvent
{
    public Guid OrderId { get; init; }

    public decimal Amount { get; init; }

    public string Currency { get; init; }


    public PaymentRequestedIntegrationEvent(
        Guid orderId,
        decimal amount,
        string currency)
    {
        OrderId = orderId;
        Amount = amount;
        Currency = currency;
    }
}