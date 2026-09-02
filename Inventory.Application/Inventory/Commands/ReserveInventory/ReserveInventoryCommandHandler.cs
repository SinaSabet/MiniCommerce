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

        var reservations = new List<InventoryReservation>();

        var alreadyReserved = true;



        foreach (var item in request.Items)
        {

            var existingReservation =
                await _reservationRepository
                    .GetByOrderAndProductAsync(
                        request.OrderId,
                        item.ProductId,
                        cancellationToken);



            if (existingReservation is not null)
            {

                _logger.LogInformation(
                    "Inventory reservation already exists. " +
                    "OrderId: {OrderId}, ProductId: {ProductId}",
                    request.OrderId,
                    item.ProductId);


                reservations.Add(existingReservation);

                continue;
            }



            alreadyReserved = false;



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



            reservations.Add(
                reservation);


            _logger.LogInformation(
                "Inventory reserved. " +
                "OrderId: {OrderId}, ProductId: {ProductId}, Quantity: {Quantity}",
                request.OrderId,
                item.ProductId,
                item.Quantity);

        }



        await _unitOfWork.SaveChangesAsync(
            cancellationToken);



        var firstReservation =
            reservations.First();



        return new ReserveInventoryCommandResponse(

            ReservationId:
                firstReservation.Id,


            OrderId:
                request.OrderId,


            ReservedItemsCount:
                reservations.Count,


            AlreadyReserved:
                alreadyReserved,


            Message:
                alreadyReserved
                ? "Inventory was already reserved."
                : "Inventory reserved successfully."

        );
    }
}