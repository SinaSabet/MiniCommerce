using MediatR;
using Ordering.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Orders.Queries
{
    public class GetOrderByIdHandler
     : IRequestHandler<GetOrderByIdQuery, OrderDto>
    {

        private readonly IOrderRepository _repository;


        public GetOrderByIdHandler(
            IOrderRepository repository)
        {
            _repository = repository;
        }



        public async Task<OrderDto> Handle(
            GetOrderByIdQuery request,
            CancellationToken cancellationToken)
        {

            var order =
                await _repository
                .GetByIdAsync(request.OrderId);



            if (order is null)
            {
                throw new Exception(
                    "Order not found");
            }



            return new OrderDto
           (
       order.Id,
       order.Status.ToString(),
       order.Items.Sum(
           x => x.UnitPrice.Amount * x.Quantity
       ),
       order.ShippingAddress.City,
       order.ShippingAddress.Street,
       order.Items
           .Select(x =>
               new OrderItemDto(
                   x.ProductId,
                   x.ProductName,
                   x.Quantity,
                   x.UnitPrice.Amount))
            .ToList()
         );

        }

    }
}
