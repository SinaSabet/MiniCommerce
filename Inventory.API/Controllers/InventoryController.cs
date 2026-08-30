using Inventory.API.Common;
using Inventory.API.Contracts;
using Inventory.Application.Inventory.Commands.AddStock;
using Inventory.Application.Inventory.Commands.ReserveInventory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;


[ApiController]
[Route("api/inventory")]
public sealed class InventoryController : ControllerBase
{
    private readonly ISender _sender;


    public InventoryController(
        ISender sender)
    {
        _sender = sender;
    }


    [HttpPost("stock")]
    public async Task<ActionResult<ApiResponse<AddStockCommandResponse>>> AddStock(
      AddStockRequest request,
      CancellationToken cancellationToken)
    {
        var command = new AddStockCommand(
            request.ProductId,
            request.Quantity);

        var result = await _sender.Send(
            command,
            cancellationToken);

        return Ok(
            ApiResponse<AddStockCommandResponse>.Ok(
                result,
                "Stock added successfully."));
    }






    [HttpPost("reserve")]
    public async Task<ActionResult<ApiResponse<ReserveInventoryCommandResponse>>>
        ReserveInventory(
            ReserveInventoryCommand request,
            CancellationToken cancellationToken)
    {


        var result =
            await _sender.Send(
                request,
                cancellationToken);


        var response =
            ApiResponse<ReserveInventoryCommandResponse>.Ok(
                result,
                result.Message);


        return Ok(response);
    }
}


