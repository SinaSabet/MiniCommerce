using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure.Saga
{
    public class OrderSagaState: SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; }

        public Guid OrderId { get; set; }

        public bool InventoryReserved { get; set; }

        public bool PaymentCompleted { get; set; }
    }
}
