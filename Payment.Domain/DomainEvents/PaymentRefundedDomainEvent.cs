using Payment.Domain.Common.Events;

namespace Payment.Domain.DomainEvents;

public sealed record PaymentRefundedDomainEvent(
    Guid PaymentId,
    Guid OrderId) : DomainEvent;
