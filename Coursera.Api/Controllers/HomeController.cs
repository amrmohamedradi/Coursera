using Coursera.Application.Features.Home.Queries.GetTopCategories;
using Coursera.Application.Features.Home.Queries.GetTopCourses;
using Coursera.Application.Features.Home.Queries.GetTopInstroctors;
using Coursera.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coursera.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IMediator _mediator;
        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("top-courses")]
        public async Task<IActionResult> GetTopCourses()
        {
            var result = await _mediator.Send(new GetTopCoursesQuery());
            return Ok(new ApiResponse<object?>(result));
        }
        [HttpGet("top-Categories")]
        public async Task<IActionResult> GetTopCategories()
        {
            var result = await _mediator.Send(new GetTopCategoriesQuery());
            return Ok(new ApiResponse<object?>(result));
        }
        [HttpGet("top-Instructor")]
        public async Task<IActionResult> GetTopInstructor()
        {
            var result = await _mediator.Send(new GetTopinstructorsQuery());
            return Ok(new ApiResponse<object?>(result));
        }
        
    }
}
