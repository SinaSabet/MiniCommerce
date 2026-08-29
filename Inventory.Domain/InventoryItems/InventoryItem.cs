using Inventory.Domain.Common.Exceptions;
using Inventory.Domain.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.InventoryItems
{
    public sealed class InventoryItem: AggregateRoot<Guid>
    {
        public Guid ProductId { get; private set; }

        public int OnHandQuantity { get; private set; }

        public int ReservedQuantity { get; private set; }


        public int AvailableQuantity =>
            OnHandQuantity - ReservedQuantity;
        private InventoryItem()
      : base(Guid.Empty)
        {
        }

        private InventoryItem(
       Guid id,
       Guid productId,
       int initialQuantity)
       : base(id)
        {
            if (productId == Guid.Empty)
                throw new DomainException(
                    "ProductId is required.");


            if (initialQuantity < 0)
                throw new DomainException(
                    "Initial quantity cannot be negative.");


            ProductId = productId;

            OnHandQuantity = initialQuantity;

            ReservedQuantity = 0;
        }


        public static InventoryItem Create(
            Guid productId,
            int initialQuantity = 0)
        {
            return new InventoryItem(
                Guid.NewGuid(),
                productId,
                initialQuantity);
        }


        public void IncreaseStock(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException(
                    "Quantity must be greater than zero.");

            OnHandQuantity += quantity;
        }


        public void Reserve(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException(
                    "Quantity must be greater than zero.");


            if (AvailableQuantity < quantity)
                throw new DomainException(
                    "Insufficient inventory.");


            ReservedQuantity += quantity;
        }


        public void Release(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException(
                    "Quantity must be greater than zero.");


            if (ReservedQuantity < quantity)
                throw new DomainException(
                    "Cannot release more than reserved quantity.");


            ReservedQuantity -= quantity;
        }


        public void CommitReservation(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException(
                    "Quantity must be greater than zero.");


            if (ReservedQuantity < quantity)
                throw new DomainException(
                    "Insufficient reserved inventory.");


            ReservedQuantity -= quantity;

            OnHandQuantity -= quantity;
        }

    }
}
