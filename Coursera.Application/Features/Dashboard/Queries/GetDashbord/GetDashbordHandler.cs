using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Dashboard.Queries.GetDashbord
{
    public class GetDashbordHandler : IRequestHandler<GetDashbordQuery,DashbordDto>
    {
        private readonly IApplicationDbContext _context;
        public GetDashbordHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashbordDto> Handle(GetDashbordQuery request, CancellationToken cancellationToken)
        {
            var coursesCount = await _context.Courses.CountAsync(cancellationToken);
            var categoriesCount = await _context.Categories.CountAsync(cancellationToken);
            var instructorsCount = await _context.Instructors.CountAsync(cancellationToken);
            var startOfManth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month,1);
            var manthySales = 0;

            return new DashbordDto
            {
                CoursesCount = coursesCount,
                CategoriesCount = categoriesCount,
                InstructorsCount = instructorsCount,
                ManthlySales = manthySales
            };
        }
    }
}
