namespace Inventory.Application.Inventory.Commands.ReserveInventory;


public sealed record ReserveInventoryCommandResponse(

    Guid ReservationId,

    IReadOnlyCollection<Guid> ReservationIds,

    Guid OrderId,

    int ReservedItemsCount,

    bool AlreadyReserved,

    string Message

);
