namespace Inventory.Application.Inventory.Commands.ReserveInventory;

public sealed record ReserveInventoryCommandResponse(
    Guid ReservationId,
    Guid OrderId,
    Guid ProductId,
    int Quantity,
    string Status,
    bool AlreadyReserved,
    string Message);