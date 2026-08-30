using Inventory.Application.Interfaces;
using Inventory.Domain.InventoryItems;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Inventory.Commands.AddStock;

public sealed class AddStockCommandHandler
    : IRequestHandler<AddStockCommand, AddStockCommandResponse>
{
    private readonly IInventoryItemRepository _inventoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddStockCommandHandler> _logger;

    public AddStockCommandHandler(
        IInventoryItemRepository inventoryRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddStockCommandHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<AddStockCommandResponse> Handle(
        AddStockCommand request,
        CancellationToken cancellationToken)
    {
        var inventoryItem =
            await _inventoryRepository.GetByProductIdAsync(
                request.ProductId,
                cancellationToken);

        var created = false;

        if (inventoryItem is null)
        {
            inventoryItem = InventoryItem.Create(
                request.ProductId,
                request.Quantity);

            await _inventoryRepository.AddAsync(
                inventoryItem,
                cancellationToken);

            created = true;
        }
        else
        {
            inventoryItem.IncreaseStock(request.Quantity);
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Stock added successfully. ProductId: {ProductId}, Quantity: {Quantity}, OnHand: {OnHand}",
            request.ProductId,
            request.Quantity,
            inventoryItem.OnHandQuantity);

        return new AddStockCommandResponse(
            InventoryItemId: inventoryItem.Id,
            ProductId: inventoryItem.ProductId,
            AddedQuantity: request.Quantity,
            OnHandQuantity: inventoryItem.OnHandQuantity,
            ReservedQuantity: inventoryItem.ReservedQuantity,
            AvailableQuantity: inventoryItem.AvailableQuantity,
            Created: created);
    }
}