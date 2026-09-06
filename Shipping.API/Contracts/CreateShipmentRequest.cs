namespace Shipping.API.Contracts;

public sealed record CreateShipmentRequest(Guid OrderId,Guid PaymentId);
