using Coursera.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
namespace Coursera.Application.Features.Courses.Commands.CreateCourse
{
    public record CreateCourseCommand(
        string Name,
        string Description,
        decimal Price,
        decimal Rate,
        string ImagePath,
        DateTime CreatedAt,
        Level Level,
        Guid CategoryId,
        Guid InstructorId
        ) : IRequest<Guid>;
}
