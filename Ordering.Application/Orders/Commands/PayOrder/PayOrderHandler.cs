using Ordering.Application.Interfaces;
using Ordering.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Orders.Commands.PayOrder
{
    public class PayOrderHandler
    {
        private readonly IOrderRepository _repository;

        private readonly IDomainEventDispatcher _dispatcher;



        public PayOrderHandler(
            IOrderRepository repository,
            IDomainEventDispatcher dispatcher)
        {
            _repository = repository;
            _dispatcher = dispatcher;
        }



        public async Task Handle(
            PayOrderCommand command, CancellationToken cancellationToken)
        {

            var order =
                await _repository
                .GetByIdAsync(command.OrderId, cancellationToken);



            if (order is null)
            {
                throw new Exception(
                    "Order not found");
            }



            // Domain Logic
            order.Pay();



            // Persistence
            await _repository
                .UpdateAsync(order);



            // Domain Events
            await _dispatcher
                .DispatchAsync(
                    order.DomainEvents, cancellationToken);



            // Cleanup
            order.ClearDomainEvents();

        }
    }
}
