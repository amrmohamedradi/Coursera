using Coursera.Application.Common.Constans;
using Coursera.Application.Common.Models;
using Coursera.Application.Features.Dashboard.Queries.GetDashbord;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coursera.Api.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class DashBoardController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DashBoardController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> GetDashcoard()
        {
            var result = await _mediator.Send(new GetDashbordQuery());
            return Ok(new ApiResponse<object?>(result));
        }
    }
}
