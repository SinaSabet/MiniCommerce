namespace Inventory.API.Contracts
{
    public sealed record AddStockRequest(
      Guid ProductId,
      int Quantity);
}
