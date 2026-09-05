namespace Inventory.Application.Inventory.Commands.ReleaseInventory;

public sealed record ReleaseInventoryCommandResponse(
    Guid OrderId,
    IReadOnlyCollection<Guid> ReservationIds,
    bool AlreadyReleased);
