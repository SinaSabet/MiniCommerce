namespace Payment.Application.Payments.Commands.FailPayment;

public sealed record FailPaymentCommandResponse(
    Guid PaymentId,
    string Message)
{
}
