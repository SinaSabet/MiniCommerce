using MediatR;

namespace Inventory.Application.Inventory.Commands.ReleaseInventory;

public sealed record ReleaseInventoryCommand(
    Guid OrderId,
    IReadOnlyCollection<Guid> ReservationIds)
    : IRequest<ReleaseInventoryCommandResponse>;
