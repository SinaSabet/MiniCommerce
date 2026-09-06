using Shipping.Domain.Common.Events;

namespace Shipping.Domain.DomainEvents;

public sealed record ShipmentFailedDomainEvent(
    Guid OrderId,
    string Reason)
    : DomainEvent;
