using MediatR;

namespace Ordering.Application.Orders.Commands.MarkInventoryReservationFailed;

public sealed record MarkInventoryReservationFailedCommand(
    Guid OrderId,
    string Reason)
    : IRequest;