using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events.Inventory;

public sealed record InventoryReservedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }

    public Guid ReservationId { get; init; }

    public IReadOnlyCollection<InventoryReservedItem> Items { get; init; }


    public InventoryReservedIntegrationEvent(
        Guid orderId,
        Guid reservationId,
        IReadOnlyCollection<InventoryReservedItem> items)
    {
        OrderId = orderId;
        ReservationId = reservationId;
        Items = items;
    }
}


public sealed record InventoryReservedItem
{
    public Guid ProductId { get; init; }

    public int Quantity { get; init; }


    public InventoryReservedItem(
        Guid productId,
        int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }
}