using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Home.Queries.GetTopInstroctors
{
    public class GetTopInstructorHandler : IRequestHandler<GetTopinstructorsQuery,List<InstructorDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetTopInstructorHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<InstructorDto>> Handle(GetTopinstructorsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Instructors
                .AsNoTracking()
                .Take(6)
                .Select(i => new InstructorDto(
                i.Id,
                i.Name,
                i.JobTitle,
                i.Bio,
                i.ImagePath)).ToListAsync(cancellationToken);
        }
    }
}
