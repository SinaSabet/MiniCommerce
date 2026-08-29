using Ordering.Application.Interfaces;
using Ordering.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Orders.Commands.CancelOrder
{
    public class CancelOrderHandler
    {
        private readonly IOrderRepository _repository;

        private readonly IDomainEventDispatcher _dispatcher;



        public CancelOrderHandler(
            IOrderRepository repository,
            IDomainEventDispatcher dispatcher)
        {
            _repository = repository;
            _dispatcher = dispatcher;
        }



        public async Task Handle(
            CancelOrderCommand command,CancellationToken cancellationToken)
        {

            var order =
                await _repository
                .GetByIdAsync(command.OrderId);



            if (order is null)
            {
                throw new Exception(
                    "Order not found");
            }



            // Domain Logic
            order.Cancel();



            // Save
            await _repository
                .UpdateAsync(order);



            // Events
            await _dispatcher
                .DispatchAsync(
                    order.DomainEvents,cancellationToken);



            // Clear
            order.ClearDomainEvents();

        }
    }
}
