using Ordering.Domain.Common.Exceptions;
using Ordering.Domain.Common.Models;
using Ordering.Domain.Events;
using Ordering.Domain.ValueObjects;


namespace Ordering.Domain.Orders;


public class Order : AggregateRoot<Guid>
{

    private readonly List<OrderItem> _items = new();


    public OrderNumber Number { get; private set; }


    public OrderStatus Status { get; private set; }


    public Address ShippingAddress { get; private set; }


    public IReadOnlyCollection<OrderItem> Items
        => _items.AsReadOnly();



    private Order()
        : base(Guid.Empty)
    {

    }



    private Order(
        Guid id,
        Address shippingAddress)
        : base(id)
    {

        Number =
            OrderNumber.Create();


        Status =
            OrderStatus.Pending;


        ShippingAddress =
            shippingAddress;

    }



    public static Order Create(Address shippingAddress)
    {

        var order =
            new Order(
                Guid.NewGuid(),
                shippingAddress);



        order.AddDomainEvent(
            new OrderCreatedEvent(
                order.Id));



        return order;

    }



    public void AddItem(
        Guid productId,
        string productName,
        Money price,
        int quantity)
    {

        OrderRules
            .CannotModifyCompletedOrder(Status);



        var item =
            new OrderItem(
                Guid.NewGuid(),
                productId,
                productName,
                price,
                quantity);



        _items.Add(item);

    }



    public void RemoveItem(
        Guid itemId)
    {

        OrderRules
            .CannotModifyCompletedOrder(Status);



        var item =
            _items.FirstOrDefault(
                x => x.Id == itemId);



        if (item == null)
            throw new DomainException(
                "Item not found");



        _items.Remove(item);

    }



    public Money CalculateTotal()
    {

        return _items
            .Select(x => x.CalculateTotal())
            .Aggregate(
                Money.Zero(),
                (total, item) =>
                    total.Add(item));

    }



    public void Confirm()
    {
        OrderRules.CannotConfirmEmptyOrder(
            _items.Count);

        if (Status != OrderStatus.Pending)
            throw new DomainException(
                "Only pending orders can be confirmed.");

        Status = OrderStatus.AwaitingInventory;


        var items =
            _items
                .Select(x =>
                    new OrderConfirmedItem(
                        x.ProductId,
                        x.Quantity))
                .ToList();


        AddDomainEvent(
            new OrderConfirmedEvent(
                Id,
                items));
    }

    public void Pay()
    {

        if (Status != OrderStatus.Paid)
            throw new DomainException(
                "Invalid order state");



        Status =
            OrderStatus.Paid;



        AddDomainEvent(
            new OrderPaidEvent(
                Id));

    }


    public void MarkInventoryReservationFailed()
    {
        if (Status != OrderStatus.AwaitingInventory)
            throw new DomainException(
                "Order must be awaiting inventory.");

        Status = OrderStatus.InventoryFailed;
    }



    public void MarkInventoryReserved()
    {
        if (Status != OrderStatus.AwaitingInventory)
            throw new DomainException(
                "Order must be awaiting inventory.");

        Status = OrderStatus.AwaitingPayment;
    }

    public void Cancel()
    {

        if (Status == OrderStatus.Completed)
            throw new DomainException(
                "Completed order cannot be cancelled");



        Status =
            OrderStatus.Cancelled;



        AddDomainEvent(
            new OrderCancelledEvent(
                Id));

    }

}