using Ordering.Domain.Common.Exceptions;
using Ordering.Domain.Common.Models;
using Ordering.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Orders
{
    public class OrderItem : Entity<Guid>
    {
        public Guid ProductId { get; private set; }


        public string ProductName { get; private set; }


        public Money UnitPrice { get; private set; }


        public int Quantity { get; private set; }

        private OrderItem()
     : base(Guid.Empty)
        {
            UnitPrice = null!;
            ProductName = null!;
        }

        internal OrderItem(
            Guid id,
            Guid productId,
            string productName,
            Money unitPrice,
            int quantity)
            : base(id)
        {

            if (quantity <= 0)
                throw new DomainException(
                    "Quantity must be greater than zero");


            ProductId = productId;

            ProductName = productName;

            UnitPrice = unitPrice;

            Quantity = quantity;

        }



        public Money CalculateTotal()
        {
            return UnitPrice.Multiply(Quantity);
        }



        internal void ChangeQuantity(int quantity)
        {

            if (quantity <= 0)
                throw new DomainException(
                    "Invalid quantity");


            Quantity = quantity;

        }

    }
}
