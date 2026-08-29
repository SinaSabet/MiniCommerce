using MediatR;
using Ordering.Domain.Orders;
using Ordering.Domain.Repositories;
using Ordering.Domain.ValueObjects;


namespace Ordering.Application.Orders.Commands.CreateOrder;


public class CreateOrderHandler
    : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{

    private readonly IOrderRepository _orderRepository;



    public CreateOrderHandler(
        IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }



    public async Task<CreateOrderResponse> Handle(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {

        var order =
            Order.Create(command.ShippingAddress);



        order.AddItem(
            command.ProductId,
            command.ProductName,
            new Money(
                command.Price,
                command.Currency),
            command.Quantity);



        await _orderRepository
            .AddAsync(order);



        return new CreateOrderResponse(
            order.Id,
            "Order created successfully"
        );

    }

}
