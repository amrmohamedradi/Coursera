using Coursera.Application.Common.Constans;
using Coursera.Application.Common.Models;
using Coursera.Application.Features.Instructors.Commands.CreateInstructor;
using Coursera.Application.Features.Instructors.Commands.DeleteInstructor;
using Coursera.Application.Features.Instructors.Commands.UpdateInstructor;
using Coursera.Application.Features.Instructors.Queries;
using Coursera.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coursera.Api.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly IMediator _mediator;
        public InstructorController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var instructor = await _mediator.Send(new GetInstructorQueryById(id));
            return Ok(new ApiResponse<object?>(instructor));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var result = await _mediator.Send(new GetInstructorQuery(pageNumber, pageSize, search));
            return Ok(new ApiResponse<object?>(result));
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateInstructorRequest request)
        {
            var jobTitle = Enum.Parse<JobTitle>(request.JobTitle);
            var id = await _mediator.Send(new CreateInstructorCommand(
                request.Name,
                jobTitle,
                request.Bio,
                request.ImagePath));
            return Ok(new ApiResponse<Guid>(id));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id,UpdateInstructorRequest request)
        {
            var jobTitle = Enum.Parse<JobTitle>(request.JobTitle);
             await _mediator.Send(new UpdateInstructorCommand(
                id,
                request.Name,
                jobTitle,
                request.Bio,
                request.ImagePath));
            return Ok(new ApiResponse<object?>(null));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteInstructorCommand(id));
            return Ok(new ApiResponse<object?>(null));
        }
    }
}
