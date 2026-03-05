using Coursera.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Common.DTOs
{
    public record CourseDto(Guid Id,
        string Name,
        string Description,
        decimal Price,
        decimal Rating,
        DateTime CreatedAt,
        Level Level,
        string ImagePath,
        Guid CategoryId,
        Guid InstructorId
        );
}
