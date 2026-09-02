using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events.Ordering;


public sealed record OrderConfirmedIntegrationEvent
    : IntegrationEvent
{

    public Guid OrderId { get; init; }


    public decimal Amount { get; init; }


    public string Currency { get; init; } = default!;


    public IReadOnlyCollection<OrderConfirmedItem> Items { get; init; }
        = new List<OrderConfirmedItem>();



    public OrderConfirmedIntegrationEvent(
        Guid orderId,
        decimal amount,
        string currency,
        IReadOnlyCollection<OrderConfirmedItem> items)
    {
        OrderId = orderId;
        Amount = amount;
        Currency = currency;
        Items = items;
    }
}



public sealed record OrderConfirmedItem(
    Guid ProductId,
    int Quantity
);