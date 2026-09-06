using Payment.Domain.Common.Exceptions;
using Payment.Domain.Common.Models;
using Payment.Domain.DomainEvents;
using Payment.Domain.ValueObjects;

namespace Payment.Domain.PaymentTransactions;

public class PaymentTransaction : AggregateRoot<Guid>
{
    public Guid OrderId { get; private set; }

    public Money Amount { get; private set; }

    public PaymentStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? CompletedAt { get; private set; }


    private PaymentTransaction()
        : base(Guid.Empty)
    {
    }


    private PaymentTransaction(
        Guid id,
        Guid orderId,
        Money amount)
        : base(id)
    {
        OrderId = orderId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        CompletedAt = null;
    }


    public static PaymentTransaction Create(
        Guid orderId,
        decimal amount,
        string currency)
    {
        if (orderId == Guid.Empty)
            throw new DomainException("OrderId cannot be empty");

        if (amount <= 0)
            throw new DomainException("Amount must be greater than zero");

        var paymentMoney = new Money(amount, currency);

        var paymentTransaction = new PaymentTransaction(
            Guid.NewGuid(),
            orderId,
            paymentMoney);

        return paymentTransaction;
    }


    public void Complete()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException(
                $"Cannot complete payment with status {Status}");

        Status = PaymentStatus.Completed;
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(
            new PaymentCompletedDomainEvent(Id, OrderId));
    }


    public void Fail(string reason)
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException(
                $"Cannot fail payment with status {Status}");

        Status = PaymentStatus.Failed;
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(
            new PaymentFailedDomainEvent(Id, OrderId, reason));
    }


    public bool Refund(decimal amount)
    {
        if (amount != Amount.Amount)
            throw new DomainException(
                $"Refund amount {amount} does not match payment amount {Amount.Amount}");

        if (Status == PaymentStatus.Refunded)
            return false;

        if (Status != PaymentStatus.Completed)
            throw new DomainException(
                $"Cannot refund payment with status {Status}");

        Status = PaymentStatus.Refunded;

        AddDomainEvent(
            new PaymentRefundedDomainEvent(Id, OrderId));

        return true;
    }


    public void RecordRefundFailure(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Refund failure reason is required");

        AddDomainEvent(
            new PaymentRefundFailedDomainEvent(Id, OrderId, reason));
    }
}
