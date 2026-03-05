using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Courses.Queries
{
    public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, CourseDto>
    {
        private readonly IApplicationDbContext _context;
        public GetCourseByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<CourseDto> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (course == null)
                throw new Exception("Course not found");
            return new CourseDto
                (
                    course.Id, course.Name, course.Description, course.Price,course.Rating,course.CreatedAt, course.Level, course.ImagePath, course.CategoryId, course.InstructorId
                );

        }
    }
}
