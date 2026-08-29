using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Orders.Commands.ConfirmOrder
{
    public record ConfirmOrderCommand(Guid OrderId)
    {
    }
}
