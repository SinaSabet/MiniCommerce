using Inventory.Domain.Common.Events;

namespace Inventory.Domain.DomainEvents;

public sealed record InventoryReservationCompletedEvent(
    Guid ReservationId,
    Guid OrderId,
    Guid ProductId,
    int Quantity)
    : DomainEvent;