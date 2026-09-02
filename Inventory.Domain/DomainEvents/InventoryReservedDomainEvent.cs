using Inventory.Domain.Common.Events;

namespace Inventory.Domain.DomainEvents;


public sealed record InventoryReservedDomainEvent(
    Guid OrderId,
    Guid ReservationId,
    IReadOnlyCollection<InventoryReservedItem> Items
)
: DomainEvent;



public sealed record InventoryReservedItem(
    Guid ProductId,
    int Quantity
);