using Coursera.Application.Common.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Courses.Queries.GetSimilarCourses
{
    public record GetSimilarCoursesQuery(Guid CourseId) : IRequest<List<CourseDto>>;

}
