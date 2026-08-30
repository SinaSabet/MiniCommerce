namespace Inventory.Application.Inventory.Commands.ReserveOrderInventory;

public sealed record ReserveOrderInventoryCommandResponse(
    Guid OrderId,
    int ReservedItemsCount,
    bool AlreadyReserved);