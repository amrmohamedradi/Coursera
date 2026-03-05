using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Courses.Queries.GetSimilarCourses
{

    public class GetSimilarCoursesHandler : IRequestHandler<GetSimilarCoursesQuery, List<CourseDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetSimilarCoursesHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CourseDto>> Handle(GetSimilarCoursesQuery request, CancellationToken cancellationToken)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == request.CourseId, cancellationToken);
            if (course == null) 
                return new List<CourseDto>();
            return await _context.Courses
                .Where(c => c.CategoryId==course.CategoryId && c.Id != request.CourseId)
                .OrderByDescending(c=> c.CreatedAt)
                .Take(4)
                .Select(i => new CourseDto(
                i.Id,
                i.Name,
                i.Description,
                i.Price,
                i.Rating,
                i.CreatedAt,
                i.Level,
                i.ImagePath,
                i.CategoryId,
                i.InstructorId)).ToListAsync(cancellationToken);
        }
    }
}
