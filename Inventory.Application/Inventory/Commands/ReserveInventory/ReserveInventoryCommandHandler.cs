using Inventory.Application.Interfaces;
using Inventory.Domain.InventoryItems;
using Inventory.Domain.Reservations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Inventory.Commands.ReserveInventory;

public sealed class ReserveInventoryCommandHandler
    : IRequestHandler<
        ReserveInventoryCommand,
        ReserveInventoryCommandResponse>
{
    private readonly IInventoryItemRepository _inventoryRepository;

    private readonly IInventoryReservationRepository _reservationRepository;

    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<ReserveInventoryCommandHandler> _logger;


    public ReserveInventoryCommandHandler(
        IInventoryItemRepository inventoryRepository,
        IInventoryReservationRepository reservationRepository,
        IUnitOfWork unitOfWork,
        ILogger<ReserveInventoryCommandHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }


    public async Task<ReserveInventoryCommandResponse> Handle(
        ReserveInventoryCommand request,
        CancellationToken cancellationToken)
    {
      
        var existingReservation =
            await _reservationRepository
                .GetByOrderAndProductAsync(
                    request.OrderId,
                    request.ProductId,
                    cancellationToken);


        if (existingReservation is not null)
        {
            _logger.LogInformation(
                "Inventory reservation already exists. " +
                "OrderId: {OrderId}, ProductId: {ProductId}, ReservationId: {ReservationId}",
                request.OrderId,
                request.ProductId,
                existingReservation.Id);


            return new ReserveInventoryCommandResponse(
                ReservationId: existingReservation.Id,
                OrderId: existingReservation.OrderId,
                ProductId: existingReservation.ProductId,
                Quantity: existingReservation.Quantity,
                Status: existingReservation.Status.ToString(),
                AlreadyReserved: true,
                Message: "Inventory was already reserved.");
        }


        /*
         * مرحله 2
         * موجودی Product را از Inventory می‌خوانیم.
         */
        var inventoryItem =
            await _inventoryRepository
                .GetByProductIdAsync(
                    request.ProductId,
                    cancellationToken);


        if (inventoryItem is null)
        {
            throw new InvalidOperationException(
                $"Inventory for product {request.ProductId} was not found.");
        }


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


        _logger.LogInformation(
            "Inventory reserved successfully. " +
            "OrderId: {OrderId}, ProductId: {ProductId}, Quantity: {Quantity}, ReservationId: {ReservationId}",
            request.OrderId,
            request.ProductId,
            request.Quantity,
            reservation.Id);


        return new ReserveInventoryCommandResponse(
            ReservationId: reservation.Id,
            OrderId: reservation.OrderId,
            ProductId: reservation.ProductId,
            Quantity: reservation.Quantity,
            Status: reservation.Status.ToString(),
            AlreadyReserved: false,
            Message: "Inventory reserved successfully.");
    }
}