using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.API.Common;
using Payment.Application.Payments.Commands.CompletePayment;
using Payment.Application.Payments.Commands.FailPayment;
using Payment.Application.Payments.Commands.ProcessPayment;

namespace Payment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("process")]
        public async Task<IActionResult> Process(
            ProcessPaymentCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(ApiResponse<ProcessPaymentCommandResponse>.Ok(
                result,
                "Payment processed successfully."));
        }

        [HttpPost("{paymentId:guid}/complete")]
        public async Task<IActionResult> Complete(
            Guid paymentId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new CompletePaymentCommand(paymentId),
                cancellationToken);

            return Ok(ApiResponse<CompletePaymentCommandResponse>.Ok(
                result,
                "Payment completed successfully."));
        }

        [HttpPost("{paymentId:guid}/fail")]
        public async Task<IActionResult> Fail(
            Guid paymentId,
            [FromBody] FailPaymentRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new FailPaymentCommand(paymentId, request.Reason),
                cancellationToken);

            return Ok(ApiResponse<FailPaymentCommandResponse>.Ok(
                result,
                "Payment failed successfully."));
        }
    }

    public class FailPaymentRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}
