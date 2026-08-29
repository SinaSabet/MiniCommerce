using MediatR;
using Ordering.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand(
    Guid ProductId,
    string ProductName,
    decimal Price,
    string Currency,
    int Quantity,
    Address ShippingAddress
):IRequest<CreateOrderResponse>;

}
