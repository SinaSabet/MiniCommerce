using MediatR;

namespace Inventory.Application.Inventory.Commands.ReserveOrderInventory;

public sealed record ReserveOrderInventoryCommand(
    Guid OrderId,
    IReadOnlyCollection<ReserveOrderInventoryItem> Items)
    : IRequest<ReserveOrderInventoryCommandResponse>;


public sealed record ReserveOrderInventoryItem(
    Guid ProductId,
    int Quantity);