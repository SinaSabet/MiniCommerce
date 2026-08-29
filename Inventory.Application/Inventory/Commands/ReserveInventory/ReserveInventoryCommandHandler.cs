using Inventory.Application.Interfaces;
using Inventory.Domain.InventoryItems;
using Inventory.Domain.Reservations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Application.Inventory.Commands.ReserveInventory
{
    public sealed class ReserveInventoryCommandHandler
     : IRequestHandler<ReserveInventoryCommand>
    {
        private readonly IInventoryItemRepository _inventoryRepository;
        private readonly IInventoryReservationRepository _reservationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReserveInventoryCommandHandler(
            IInventoryItemRepository inventoryRepository,
            IInventoryReservationRepository reservationRepository,
            IUnitOfWork unitOfWork)
        {
            _inventoryRepository = inventoryRepository;
            _reservationRepository = reservationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            ReserveInventoryCommand request,
            CancellationToken cancellationToken)
        {
            // فعلاً یک محافظ ساده در برابر اجرای دوباره
            var existingReservation =
                await _reservationRepository
                    .GetByOrderAndProductAsync(
                        request.OrderId,
                        request.ProductId,
                        cancellationToken);

            if (existingReservation is not null)
                return;


            var inventoryItem =
                await _inventoryRepository
                    .GetByProductIdAsync(
                        request.ProductId,
                        cancellationToken);

            if (inventoryItem is null)
                throw new InvalidOperationException(
                    $"Inventory for product {request.ProductId} was not found.");


            inventoryItem.Reserve(
                request.Quantity);


            var reservation =
                InventoryReservation.Create(
                    request.OrderId,
                    request.ProductId,
                    request.Quantity);


            await _reservationRepository.AddAsync(
                reservation,
                cancellationToken);


            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }
    }
}
