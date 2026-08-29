using Ordering.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Orders.Commands.ConfirmOrder
{
    public class ConfirmOrderHandler
    {
        private readonly IOrderRepository _repository;

        public ConfirmOrderHandler(
            IOrderRepository repository)
        {
            _repository = repository;
        }



        public async Task Handle(
            ConfirmOrderCommand command)
        {

            var order =
                await _repository
                .GetByIdAsync(command.OrderId);



            if (order is null)
            {
                throw new Exception(
                    "Order not found");
            }



            order.Confirm();



            await _repository
                .UpdateAsync(order);

        }
    }
}
