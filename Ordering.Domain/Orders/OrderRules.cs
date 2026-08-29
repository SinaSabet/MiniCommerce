using Ordering.Domain.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Orders
{
    public static class OrderRules
    {
        public static void
      CannotConfirmEmptyOrder(
      int itemCount)
        {

            if (itemCount == 0)
            {
                throw new DomainException(
                    "Order cannot be confirmed without items");
            }

        }



        public static void
            CannotModifyCompletedOrder(
            OrderStatus status)
        {

            if (status == OrderStatus.Completed ||
               status == OrderStatus.Cancelled)
            {
                throw new DomainException(
                    "Order cannot be modified");
            }

        }
    }
}
