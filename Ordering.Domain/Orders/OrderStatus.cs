using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Orders
{
    public enum OrderStatus
    {
        Pending = 1,

        Confirmed = 2,

        Paid = 3,

        Shipped = 4,

        Completed = 5,

        Cancelled = 6
    }
}
