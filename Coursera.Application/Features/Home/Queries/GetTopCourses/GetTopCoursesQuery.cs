using Coursera.Application.Common.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Home.Queries.GetTopCourses
{
    public record GetTopCoursesQuery() : IRequest<List<CourseDto>>;
}
