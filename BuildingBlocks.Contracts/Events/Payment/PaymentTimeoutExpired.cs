namespace BuildingBlocks.Contracts.Events.Payment;

public sealed record PaymentTimeoutExpired(Guid OrderId);
