using Shipping.Domain.Common.Events;

namespace Shipping.Domain.DomainEvents;

public sealed record ShipmentCreatedDomainEvent(
    Guid OrderId,
    Guid PaymentId,
    Guid ShipmentId)
    : DomainEvent;
