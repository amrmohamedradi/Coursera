using Coursera.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Common.DTOs
{
    public record CategoryDto(Guid Id,
        string Name,
        string ImagePath);
}
