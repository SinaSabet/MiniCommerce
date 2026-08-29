using Ordering.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Specifications
{
    public interface IOrderSpecification
    {
        bool IsSatisfiedBy(Order order);

    }
}
