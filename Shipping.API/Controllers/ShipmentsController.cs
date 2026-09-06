using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shipping.API.Contracts;
using Shipping.Application.Shipments;
using Shipping.Application.Shipments.Commands.CreateShipment;
using Shipping.Application.Shipments.Queries.GetShipment;

namespace Shipping.API.Controllers;

[ApiController]
[Route("api/shipments")]
public sealed class ShipmentsController : ControllerBase
{
    private readonly ISender _sender;

    public ShipmentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<ActionResult<ShipmentDto>> Create(
        CreateShipmentRequest request,
        CancellationToken cancellationToken)
    {
        var shipment = await _sender.Send(
            new CreateShipmentCommand(request.OrderId , request.PaymentId),
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { shipmentId = shipment.Id },
            shipment);
    }

    [HttpGet("{shipmentId:guid}")]
    public async Task<ActionResult<ShipmentDto>> GetById(
        Guid shipmentId,
        CancellationToken cancellationToken)
    {
        var shipment = await _sender.Send(
            new GetShipmentQuery(shipmentId),
            cancellationToken);

        return shipment is null
            ? NotFound()
            : Ok(shipment);
    }
}
