using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Orders.Commands.CreateOrder;
using Ordering.Application.Orders.Queries;

namespace Ordering.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;



        public OrdersController(
            IMediator mediator)
        {
            _mediator = mediator;
        }



        [HttpPost]
        public async Task<IActionResult> Create(
            CreateOrderCommand command)
        {

            var orderId =
                await _mediator.Send(command);


            return Ok(orderId);

        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
    Guid id)
        {

            var result =
                await _mediator.Send(
                    new GetOrderByIdQuery(id));


            return Ok(result);

        }
    }
}
