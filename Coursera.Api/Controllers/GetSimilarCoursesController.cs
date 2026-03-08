using Coursera.Application.Features.Courses.Queries.GetSimilarCourses;
using Coursera.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Coursera.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetSimilarCoursesController : Controller
    {
        private readonly IMediator _mediator;
        public GetSimilarCoursesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{id}/similar")]
        public async Task<IActionResult> GetSimilarCourses(Guid Id)
        {
            var result = await _mediator.Send(new GetSimilarCoursesQuery(Id));

            return Ok(new ApiResponse<object?>(result));

        }

    }
}
