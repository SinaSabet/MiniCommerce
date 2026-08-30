using MediatR;
using Ordering.Domain.Orders;
using Ordering.Domain.Repositories;

namespace Ordering.Application.Orders.Commands.MarkInventoryReserved;

public sealed class MarkInventoryReservedCommandHandler
    : IRequestHandler<MarkInventoryReservedCommand>
{
    private readonly IOrderRepository _orderRepository;


    public MarkInventoryReservedCommandHandler(
        IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }


    public async Task Handle(
        MarkInventoryReservedCommand request,
        CancellationToken cancellationToken)
    {
        var order =
            await _orderRepository.GetByIdAsync(
                request.OrderId,
                cancellationToken);


        if (order is null)
            throw new InvalidOperationException(
                $"Order {request.OrderId} was not found.");


        order.MarkInventoryReserved();
    }
}