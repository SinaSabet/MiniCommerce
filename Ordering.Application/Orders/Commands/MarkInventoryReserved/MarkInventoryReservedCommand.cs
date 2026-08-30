using MediatR;

namespace Ordering.Application.Orders.Commands.MarkInventoryReserved;

public sealed record MarkInventoryReservedCommand(
    Guid OrderId)
    : IRequest;