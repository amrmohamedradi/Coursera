using Coursera.Application.Features.Carts.Commands.AddToCart;
using Coursera.Application.Features.Carts.Commands.RemoveCart;
using Coursera.Application.Features.Carts.Queries.GetCart;
using Coursera.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Coursera.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("UserId not found in token"));
            var result = await _mediator.Send(new GetCartQuery(userId));
            return Ok(new ApiResponse<object?>(result));
        }
        [HttpPost("{courseId}")]
        public async Task<IActionResult> AddToCart(Guid courseId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                ?? throw new UnauthorizedAccessException("UserId not found in token"));
            await _mediator.Send(new AddToCartCommand(courseId, userId));
            return Ok(new ApiResponse<object?>());
        }
        [HttpDelete("{courseId}")]
        public async Task<IActionResult> RemoveFromCart(Guid courseId)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                ?? throw new UnauthorizedAccessException("UserId not found in token"));
            await _mediator.Send(new RemoveCartCommand(userId, courseId));
            return Ok(new ApiResponse<object?>());
        }
    }
}
