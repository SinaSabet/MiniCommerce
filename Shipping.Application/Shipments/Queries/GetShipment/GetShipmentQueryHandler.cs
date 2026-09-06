using MediatR;
using Shipping.Domain.Shipments;

namespace Shipping.Application.Shipments.Queries.GetShipment;

public sealed class GetShipmentQueryHandler
    : IRequestHandler<GetShipmentQuery, ShipmentDto?>
{
    private readonly IShipmentRepository _repository;

    public GetShipmentQueryHandler(IShipmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<ShipmentDto?> Handle(
        GetShipmentQuery request,
        CancellationToken cancellationToken)
    {
        var shipment = await _repository.GetByIdAsync(
            request.ShipmentId,
            cancellationToken);

        return shipment is null
            ? null
            : ShipmentDto.FromShipment(shipment);
    }
}
