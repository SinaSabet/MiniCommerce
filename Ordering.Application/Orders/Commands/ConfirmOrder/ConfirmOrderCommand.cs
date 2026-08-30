using MediatR;

namespace Ordering.Application.Orders.Commands.ConfirmOrder;

public sealed record ConfirmOrderCommand(
    Guid OrderId)
    : IRequest<ConfirmOrderCommandResponse>;