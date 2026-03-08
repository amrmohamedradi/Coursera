using Coursera.Application.Common.Constans;
using Coursera.Application.Common.Models;
using Coursera.Application.Features.Categories.Commands.CreateCategory;
using Coursera.Application.Features.Categories.Commands.DeleteCategory;
using Coursera.Application.Features.Categories.Commands.UpdateCategory;
using Coursera.Application.Features.Categories.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coursera.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var course =  await _mediator.Send(new GetCategoryByIdQuery(id));
            return Ok(new ApiResponse<object?>(course));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var course = await _mediator.Send(new GetCategoriesQuery());
            return Ok(new ApiResponse<object?>(course));
        }
        [Authorize (Roles =Roles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryRequest request)
        {
            var course = await _mediator.Send(new CreateCategoryCommand(
                request.Name,
                request.ImagePath));
            return Ok(new ApiResponse<object?>(course));
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid Id,UpdateCategoryRequest request)
        {
            var course = await _mediator.Send(new UpdateCategoryCommand(
                Id,
                request.Name,
                request.ImagePath));
            return Ok(new ApiResponse<object?>(course));
        }
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var course = await _mediator.Send(new DeleteCategoryCommand(
                Id));
            return Ok(new ApiResponse<object?>(course));
        }
    }
}
