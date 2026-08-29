using Inventory.Domain.Common.Exceptions;
using Inventory.Domain.Common.Models;
using Inventory.Domain.DomainEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.Reservations
{
    public sealed class InventoryReservation : AggregateRoot<Guid>
    {
        public Guid OrderId { get; private set; }

        public Guid ProductId { get; private set; }

        public int Quantity { get; private set; }

        public ReservationStatus Status { get; private set; }
        private InventoryReservation(Guid id) : base(id)
        {
        }


        private InventoryReservation(
       Guid id,
       Guid orderId,
       Guid productId,
       int quantity)
       : base(id)
        {
            if (orderId == Guid.Empty)
                throw new DomainException(
                    "OrderId is required.");


            if (productId == Guid.Empty)
                throw new DomainException(
                    "ProductId is required.");


            if (quantity <= 0)
                throw new DomainException(
                    "Quantity must be greater than zero.");


            OrderId = orderId;

            ProductId = productId;

            Quantity = quantity;

            Status = ReservationStatus.Reserved;
        }


        public static InventoryReservation Create(
            Guid orderId,
            Guid productId,
            int quantity)
        {
            var reservation =
                new InventoryReservation(
                    Guid.NewGuid(),
                    orderId,
                    productId,
                    quantity);


            reservation.AddDomainEvent(
                new InventoryReservedEvent(
                    reservation.Id,
                    orderId,
                    productId,
                    quantity));


            return reservation;
        }


        public void Release()
        {
            if (Status != ReservationStatus.Reserved)
                throw new DomainException(
                    "Only reserved inventory can be released.");


            Status = ReservationStatus.Released;


            AddDomainEvent(
                new InventoryReservationReleasedEvent(
                    Id,
                    OrderId,
                    ProductId,
                    Quantity));
        }


        public void Complete()
        {
            if (Status != ReservationStatus.Reserved)
                throw new DomainException(
                    "Only reserved inventory can be completed.");


            Status = ReservationStatus.Completed;


            AddDomainEvent(
                new InventoryReservationCompletedEvent(
                    Id,
                    OrderId,
                    ProductId,
                    Quantity));
        }
    }
}
