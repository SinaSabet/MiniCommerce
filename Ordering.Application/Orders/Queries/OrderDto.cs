using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Orders.Queries
{
    public record OrderDto
 (
     Guid Id,
     string Status,
     decimal TotalAmount,
     string City,
     string Street,
     List<OrderItemDto> Items
 );


    public record OrderItemDto
    (
        Guid ProductId,
        string ProductName,
        int Quantity,
        decimal Price
    );
}
