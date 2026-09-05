using BuildingBlocks.Contracts.Events.Inventory;
using Inventory.Application.Interfaces;
using Inventory.Domain.InventoryItems;
using Inventory.Domain.Reservations;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Inventory.Commands.ReleaseInventory;

public sealed class ReleaseInventoryCommandHandler
    : IRequestHandler<
        ReleaseInventoryCommand,
        ReleaseInventoryCommandResponse>
{
    private readonly IInventoryItemRepository _inventoryRepository;
    private readonly IInventoryReservationRepository _reservationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<ReleaseInventoryCommandHandler> _logger;

    public ReleaseInventoryCommandHandler(
        IInventoryItemRepository inventoryRepository,
        IInventoryReservationRepository reservationRepository,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint,
        ILogger<ReleaseInventoryCommandHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _reservationRepository = reservationRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<ReleaseInventoryCommandResponse> Handle(
        ReleaseInventoryCommand request,
        CancellationToken cancellationToken)
    {
        var reservationIds =
            request.ReservationIds
                .Where(x => x != Guid.Empty)
                .Distinct()
                .ToList();

        if (reservationIds.Count == 0)
        {
            throw new ArgumentException(
                "At least one reservation identifier is required.",
                nameof(request));
        }

        var reservations = new List<InventoryReservation>();

        foreach (var reservationId in reservationIds)
        {
            var reservation =
                await _reservationRepository.GetByIdAsync(
                    reservationId,
                    cancellationToken);

            if (reservation is null)
            {
                throw new KeyNotFoundException(
                    $"Inventory reservation {reservationId} was not found.");
            }

            if (reservation.OrderId != request.OrderId)
            {
                throw new InvalidOperationException(
                    $"Inventory reservation {reservationId} does not belong to order {request.OrderId}.");
            }

            reservations.Add(reservation);
        }

        var reservationsToRelease =
            reservations
                .Where(x => x.Status != ReservationStatus.Released)
                .ToList();

        var inventoryItems = new Dictionary<Guid, InventoryItem>();

        foreach (var reservation in reservationsToRelease)
        {
            var inventoryItem =
                await _inventoryRepository.GetByProductIdAsync(
                    reservation.ProductId,
                    cancellationToken);

            if (inventoryItem is null)
            {
                throw new InvalidOperationException(
                    $"Inventory for product {reservation.ProductId} was not found.");
            }

            inventoryItems.Add(
                reservation.Id,
                inventoryItem);
        }

        foreach (var reservation in reservationsToRelease)
        {
            reservation.Release();
            inventoryItems[reservation.Id].Release(
                reservation.Quantity);
        }


        await _publishEndpoint.Publish(
            new InventoryReleasedIntegrationEvent
            {
                OrderId = request.OrderId,
                ReservationIds = reservationIds.ToArray(),
                ReservationId = reservationIds[0]
            },
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Inventory reservations released. OrderId: {OrderId}, ReservationCount: {ReservationCount}, AlreadyReleased: {AlreadyReleased}",
            request.OrderId,
            reservationIds.Count,
            reservationsToRelease.Count == 0);

        return new ReleaseInventoryCommandResponse(
            request.OrderId,
            reservationIds,
            AlreadyReleased: reservationsToRelease.Count == 0);
    }
}
