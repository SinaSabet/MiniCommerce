using Ordering.Domain.Orders;
using Ordering.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Services
{
    public class OrderPricingService
    {
        public Money CalculateFinalPrice(
      Order order)
        {

            var subtotal =
                order.CalculateTotal();


            var discount =
                CalculateDiscount(order);



            return subtotal.Add(
                discount);

        }

        private Money CalculateDiscount(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
