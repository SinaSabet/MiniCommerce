using Ordering.Domain.Common.Events;

namespace Ordering.Domain.Events;


public sealed record OrderConfirmedEvent(
    Guid OrderId,
    decimal Amount,
    string Currency,
    IReadOnlyCollection<OrderConfirmedItem> Items
)
: DomainEvent;



public sealed record OrderConfirmedItem(
    Guid ProductId,
    int Quantity
);