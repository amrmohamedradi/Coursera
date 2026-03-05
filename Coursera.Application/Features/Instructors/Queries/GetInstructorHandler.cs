using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Interfaces;
using Coursera.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Instructors.Queries
{
    public class GetInstructorHandler : IRequestHandler<GetInstructorQuery, PaginatedList<InstructorDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetInstructorHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<PaginatedList<InstructorDto>> Handle(GetInstructorQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Instructors.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(i => i.Name.Contains(request.Search));
            }
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.OrderByDescending(i => i.Name).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).Select(i => new InstructorDto(
                i.Id,
                i.Name,
                i.JobTitle,
                i.Bio,
                i.ImagePath)).ToListAsync(cancellationToken);

            return new PaginatedList<InstructorDto>(items,totalCount,request.PageNumber,request.PageSize);
        }
    }
}
