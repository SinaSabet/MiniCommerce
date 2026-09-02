namespace Inventory.Application.Inventory.Commands.ReserveInventory;


public sealed record ReserveInventoryCommandResponse(

    Guid ReservationId,

    Guid OrderId,

    int ReservedItemsCount,

    bool AlreadyReserved,

    string Message

);