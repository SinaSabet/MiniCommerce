namespace Inventory.Application.Inventory.Commands.AddStock;

public sealed record AddStockCommandResponse(
    Guid InventoryItemId,
    Guid ProductId,
    int AddedQuantity,
    int OnHandQuantity,
    int ReservedQuantity,
    int AvailableQuantity,
    bool Created);