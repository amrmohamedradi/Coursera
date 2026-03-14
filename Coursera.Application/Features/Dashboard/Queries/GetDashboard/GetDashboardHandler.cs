using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Dashboard.Queries.GetDashboard
{
    public class GetDashboardHandler : IRequestHandler<GetDashboardQuery,DashboardDto>
    {
        private readonly IApplicationDbContext _context;
        public GetDashboardHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        {
            var coursesCount = await _context.Courses.CountAsync(cancellationToken);
            var categoriesCount = await _context.Categories.CountAsync(cancellationToken);
            var instructorsCount = await _context.Instructors.CountAsync(cancellationToken);
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month,1);
            var monthlySales = 0;

            return new DashboardDto
            {
                CoursesCount = coursesCount,
                CategoriesCount = categoriesCount,
                InstructorsCount = instructorsCount,
                MonthlySales = monthlySales
            };
        }
    }
}
