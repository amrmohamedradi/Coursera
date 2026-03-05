using Coursera.Application.Features.Carts.Commands.RemoveCart;
using Coursera.Application.Features.Carts.Queries.GetCart;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            var userId = Guid.Parse(User.FindFirst("uid")!.Value);
            var result = await _mediator.Send(new GetCartQuery(userId));
            return Ok(result);
        }
        [HttpDelete("{courseId}")]
        public async Task<IActionResult> RemoveFromCart(Guid courseId)
        {
            var userId = Guid.Parse(User.FindFirst("uid")!.Value);
            await _mediator.Send(new RemoveCartCommand(userId,courseId));
            return NoContent();
        }
    }
}
