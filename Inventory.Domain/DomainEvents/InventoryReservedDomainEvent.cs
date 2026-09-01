using Inventory.Domain.Common.Events;

namespace Inventory.Domain.DomainEvents;

public sealed record InventoryReservedDomainEvent(
    Guid OrderId,
    Guid ReservationId,
    IReadOnlyCollection<InventoryReservedDomainItem> Items
) : DomainEvent;


public sealed record InventoryReservedDomainItem(
    Guid ProductId,
    int Quantity
);