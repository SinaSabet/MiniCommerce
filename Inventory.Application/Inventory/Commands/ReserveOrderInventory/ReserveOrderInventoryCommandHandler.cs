using Inventory.Application.Interfaces;
using Inventory.Domain.InventoryItems;
using Inventory.Domain.Reservations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Inventory.Commands.ReserveOrderInventory;

public sealed class ReserveOrderInventoryCommandHandler
    : IRequestHandler<
        ReserveOrderInventoryCommand,
        ReserveOrderInventoryCommandResponse>
{
    private readonly IInventoryItemRepository _inventoryRepository;
    private readonly IInventoryReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReserveOrderInventoryCommandHandler> _logger;


    public ReserveOrderInventoryCommandHandler(
        IInventoryItemRepository inventoryRepository,
        IInventoryReservationRepository reservationRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReserveOrderInventoryCommandHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }


    public async Task<ReserveOrderInventoryCommandResponse> Handle(
        ReserveOrderInventoryCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Items.Count == 0)
            throw new InvalidOperationException(
                "Order does not contain any inventory items.");


        var reservations = new List<InventoryReservation>();


        foreach (var item in request.Items)
        {
            // Idempotency اولیه
            var existingReservation =
                await _reservationRepository
                    .GetByOrderAndProductAsync(
                        request.OrderId,
                        item.ProductId,
                        cancellationToken);


            if (existingReservation is not null)
            {
                continue;
            }


            var inventoryItem =
                await _inventoryRepository
                    .GetByProductIdAsync(
                        item.ProductId,
                        cancellationToken);


            if (inventoryItem is null)
            {
                throw new InvalidOperationException(
                    $"Inventory for product {item.ProductId} was not found.");
            }


            inventoryItem.Reserve(
                item.Quantity);


            var reservation =
                InventoryReservation.Create(
                    request.OrderId,
                    item.ProductId,
                    item.Quantity);


            await _reservationRepository.AddAsync(
                reservation,
                cancellationToken);


            reservations.Add(reservation);
        }


       
        if (reservations.Count > 0)
        {
            await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        }


        var alreadyReserved =
            reservations.Count == 0;


        _logger.LogInformation(
            "Inventory reservation completed for OrderId: {OrderId}. ReservedItems: {Count}",
            request.OrderId,
            reservations.Count);


        return new ReserveOrderInventoryCommandResponse(
            OrderId: request.OrderId,
            ReservedItemsCount: request.Items.Count,
            AlreadyReserved: alreadyReserved);
    }
}