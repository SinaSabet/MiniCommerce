using BuildingBlocks.Messaging;
using System.Text.Json.Serialization;

namespace BuildingBlocks.Contracts.Events.Inventory;

public sealed record InventoryReservedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }

    public Guid ReservationId { get; init; }

    public IReadOnlyCollection<Guid> ReservationIds { get; init; }
        = Array.Empty<Guid>();

    public IReadOnlyCollection<InventoryReservedItem> Items { get; init; }


    public InventoryReservedIntegrationEvent(
        Guid orderId,
        Guid reservationId,
        IReadOnlyCollection<InventoryReservedItem> items)
        : this(
            orderId,
            new[] { reservationId },
            items)
    {
    }


    [JsonConstructor]
    public InventoryReservedIntegrationEvent(
        Guid orderId,
        IReadOnlyCollection<Guid> reservationIds,
        IReadOnlyCollection<InventoryReservedItem> items)
    {
        OrderId = orderId;
        ReservationIds = reservationIds.Distinct().ToArray();
        ReservationId = ReservationIds.FirstOrDefault();
        Items = items.ToArray();
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
