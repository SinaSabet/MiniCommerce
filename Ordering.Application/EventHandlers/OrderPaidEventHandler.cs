using Microsoft.Extensions.Logging;
using Ordering.Application.Interfaces;
using Ordering.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.EventHandlers
{
    public class OrderPaidEventHandler
        : IDomainEventHandler<OrderPaidEvent>
    {

        private readonly ILogger<OrderPaidEventHandler> _logger;


        public OrderPaidEventHandler(
            ILogger<OrderPaidEventHandler> logger)
        {
            _logger = logger;
        }



        public Task HandleAsync(
            OrderPaidEvent domainEvent,
            CancellationToken cancellationToken)
        {

            _logger.LogInformation(
                "Order paid event handled successfully. OrderId: {OrderId}",
                domainEvent.OrderId);



            return Task.CompletedTask;
        }

    }
}
