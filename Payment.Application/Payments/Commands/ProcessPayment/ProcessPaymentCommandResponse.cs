namespace Payment.Application.Payments.Commands.ProcessPayment;

public sealed record ProcessPaymentCommandResponse(
    Guid PaymentId,
    string Message)
{
}
