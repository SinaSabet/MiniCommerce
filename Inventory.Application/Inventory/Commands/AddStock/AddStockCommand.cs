using MediatR;

namespace Inventory.Application.Inventory.Commands.AddStock;

public sealed record AddStockCommand(
    Guid ProductId,
    int Quantity)
    : IRequest<AddStockCommandResponse>;