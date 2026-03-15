using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Interfaces;
using Coursera.Application.Common.Models;
using Microsoft.EntityFrameworkCore;
using Coursera.Application.Features.Instructors.Queries;
using System;
using System.Collections.Generic;
using System.Text;
using Coursera.Domain.Entities;
using MediatR;

namespace Coursera.Application.Features.Courses.Queries
{
    public class GetCourseQueryHandler : IRequestHandler<GetCourseQuery, PaginatedList<CourseDto>> 
    {
        private readonly IApplicationDbContext _context;
        public GetCourseQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PaginatedList<CourseDto>> Handle(GetCourseQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Courses.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Include(i => i.Category).Where(i => 
                    i.Name.ToLower().Contains(search) || 
                    i.Category.Name.ToLower().Contains(search) ||
                    i.Price.ToString().Contains(search) ||
                    i.Rating.ToString().Contains(search));
            }
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.OrderByDescending(i => i.Name)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
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

            return new PaginatedList<CourseDto>(items, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
