using Ordering.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Specifications
{
    public class OrderCanBePaidSpecification: IOrderSpecification
    {
        public bool IsSatisfiedBy(Order order)
        {
            return order.Status == OrderStatus.Confirmed;
        }
    }
}
