using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Home.Queries.GetTopCourses
{
    public class GetTopCoursesHandler : IRequestHandler<GetTopCoursesQuery, List<CourseDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetTopCoursesHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        

        public async Task<List<CourseDto>> Handle(GetTopCoursesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Courses.AsNoTracking().OrderByDescending(c => c.CreatedAt)
                .Take(6)
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
