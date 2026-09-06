using Payment.Domain.Common.Events;

namespace Payment.Domain.DomainEvents;

public sealed record PaymentRefundFailedDomainEvent(
    Guid PaymentId,
    Guid OrderId,
    string Reason) : DomainEvent;
