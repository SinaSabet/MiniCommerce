using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Inventory.Commands.ReserveInventory
{
    public sealed record ReserveInventoryCommand(
     Guid OrderId,
     Guid ProductId,
     int Quantity)
     : IRequest;
}
