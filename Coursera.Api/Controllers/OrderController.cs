using Coursera.Application.Features.Orders.Commands.Checkout;
using Coursera.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? throw new UnauthorizedAccessException("UserId not found in token")); var orderId = await _mediator.Send(new CheckoutCommand(userId));
            return Ok(new ApiResponse<Guid>(orderId));
        }
        [HttpGet("Success")]
        public async Task<IActionResult> PaymentSuccess()
        {
            return Ok(new ApiResponse<object?>(new
            {
                message = "Payment completed successfully"
            }));
        }
    }
}
