using Coursera.Application.Common.Constans;
using Coursera.Application.Common.Models;
using Coursera.Application.Features.Courses.Commands.CreateCourse;
using Coursera.Application.Features.Courses.Commands.DeleteCourse;
using Coursera.Application.Features.Courses.Commands.UpdateCourse;
using Coursera.Application.Features.Courses.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coursera.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CourseController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid Id)
        {
            var course = await _mediator.Send(new GetCourseByIdQuery(Id));
            return Ok(new ApiResponse<object?>(course));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var result = await _mediator.Send(new GetCourseQuery(pageNumber, pageSize, search));
            return Ok(new ApiResponse<object?>(result)); 
        }
        [Authorize(Roles = Roles.Admin)]

        [HttpPost]
        public async Task<IActionResult> Create(CreateCourseRequest request)
        {
            var id = await _mediator.Send(new CreateCourseCommand(
                request.Name,
                request.Description,
                request.Price,
                request.Rating,
                request.ImagePath,
                request.CreatedAt,
                request.Level,
                request.CategoryId,
                request.InstructorId));
            return Ok(new ApiResponse<Guid>(id));
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateCourseRequest request)
        {
            
            await _mediator.Send(new UpdateCourseCommand(
                id,
                request.Name,
                request.Description,
                request.Price,
                request.Rating,
                request.ImagePath,
                request.CreatedAt,
                request.Level,
                request.CategoryId,
                request.InstructorId));
            return Ok(new ApiResponse<object?>());
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteCourseCommand(id));
            return Ok(new ApiResponse<object?>());
        }
    }
}
