using MediatR;

namespace Inventory.Application.Inventory.Commands.ReserveInventory;


public sealed record ReserveInventoryCommand(
    Guid OrderId,
    IReadOnlyCollection<ReserveInventoryCommandItem> Items
)
: IRequest<ReserveInventoryCommandResponse>;



public sealed record ReserveInventoryCommandItem(
    Guid ProductId,
    int Quantity
);