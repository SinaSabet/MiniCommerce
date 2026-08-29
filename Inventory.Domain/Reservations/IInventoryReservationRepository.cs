using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.Reservations
{
    public interface IInventoryReservationRepository
    {
        Task<InventoryReservation?> GetByIdAsync(
       Guid reservationId,
       CancellationToken cancellationToken = default);


        Task<InventoryReservation?> GetByOrderAndProductAsync(
            Guid orderId,
            Guid productId,
            CancellationToken cancellationToken = default);


        Task AddAsync(
            InventoryReservation reservation,
            CancellationToken cancellationToken = default);
    }
}
