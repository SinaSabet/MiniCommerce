using BuildingBlocks.Messaging;

namespace BuildingBlocks.Contracts.Events.Inventory;


public sealed record ReserveInventoryRequestedIntegrationEvent
    : IntegrationEvent
{

    public Guid OrderId { get; init; }


    public IReadOnlyCollection<ReserveInventoryItem> Items { get; init; }
        = new List<ReserveInventoryItem>();

}



public sealed record ReserveInventoryItem(
    Guid ProductId,
    int Quantity
);