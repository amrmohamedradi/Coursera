using Coursera.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Common.DTOs
{
    public record InstructorDto(Guid Id,
        string Name,
        JobTitle JobTitle,
        string Bio,
        string ImagePath
        );
}
