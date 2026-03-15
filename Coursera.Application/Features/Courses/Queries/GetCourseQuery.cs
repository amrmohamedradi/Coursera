using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Courses.Queries
{
    public record GetCourseQuery(int PageNumber = 1, int PageSize = 10, string? Search = null) : IRequest<PaginatedList<CourseDto>>;
}
