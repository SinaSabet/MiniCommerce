using MediatR;
using Ordering.Domain.Orders;
using Microsoft.Extensions.Logging;
using Ordering.Domain.Repositories;

namespace Ordering.Application.Orders.Commands.MarkInventoryReservationFailed;

public sealed class MarkInventoryReservationFailedCommandHandler
    : IRequestHandler<MarkInventoryReservationFailedCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<MarkInventoryReservationFailedCommandHandler> _logger;

    public MarkInventoryReservationFailedCommandHandler(
        IOrderRepository orderRepository,
        ILogger<MarkInventoryReservationFailedCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }


    public async Task Handle(
        MarkInventoryReservationFailedCommand request,
        CancellationToken cancellationToken)
    {
        var order =
            await _orderRepository.GetByIdAsync(
                request.OrderId,
                cancellationToken);

        if (order is null)
            throw new InvalidOperationException(
                $"Order {request.OrderId} was not found.");


        order.MarkInventoryReservationFailed();


        _logger.LogWarning(
            "Inventory reservation failed for OrderId: {OrderId}. Reason: {Reason}",
            request.OrderId,
            request.Reason);
    }
}