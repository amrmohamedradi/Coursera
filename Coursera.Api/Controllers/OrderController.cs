using Coursera.Application.Features.Orders.Commands.Checkout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coursera.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout()
        {
            var userId = Guid.Parse(User.FindFirst("uid").Value);
            var orderId = await _mediator.Send(new CheckoutCommand(userId));
            return Ok(orderId);
        }
        [HttpGet("Seccess")]
        public async Tak<IActionResult> PaymentSuccess()
        {
            return Ok(new 
            {
                message = "Payment completed successfully"
            });
        }
    }
}
