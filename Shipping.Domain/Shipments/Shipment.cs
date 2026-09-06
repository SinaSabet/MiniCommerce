using Shipping.Domain.Common.Exceptions;
using Shipping.Domain.Common.Models;
using Shipping.Domain.DomainEvents;

namespace Shipping.Domain.Shipments;

public sealed class Shipment : AggregateRoot<Guid>
{
    private Shipment()
        : base(Guid.Empty)
    {
    }

    private Shipment(
        Guid id,
        Guid orderId,
        Guid paymentId,
        DateTime createdAt)
        : base(id)
    {
        if (orderId == Guid.Empty)
            throw new DomainException("OrderId is required.");

        if (paymentId == Guid.Empty)
            throw new DomainException("PaymentId is required.");

        OrderId = orderId;
        PaymentId = paymentId;
        Status = ShipmentStatus.Pending;
        CreatedAt = createdAt;
    }

    public Guid OrderId { get; private set; }

    public Guid PaymentId { get; private set; }

    public ShipmentStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ShippedAt { get; private set; }

    public static Shipment Create(Guid orderId, Guid paymentId)
    {
        return new Shipment(
            Guid.NewGuid(),
            orderId,
            paymentId,
            DateTime.UtcNow);
    }

    public void MarkCreated()
    {
        if (Status != ShipmentStatus.Pending)
            throw new DomainException("Only pending shipments can be created.");

        Status = ShipmentStatus.Created;

        AddDomainEvent(
            new ShipmentCreatedDomainEvent(
                OrderId,
                PaymentId,
                Id));
    }

    public void MarkFailed(string reason)
    {
        if (Status != ShipmentStatus.Pending)
            throw new DomainException("Only pending shipments can fail.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Failure reason is required.");

        Status = ShipmentStatus.Failed;

        AddDomainEvent(
            new ShipmentFailedDomainEvent(
                OrderId,
                reason));
    }

    public void RecordShippedAt(DateTime shippedAt)
    {
        if (Status != ShipmentStatus.Created)
            throw new DomainException("Only created shipments can be shipped.");

        if (shippedAt == default)
            throw new DomainException("ShippedAt is required.");

        ShippedAt = shippedAt.ToUniversalTime();
    }
}
