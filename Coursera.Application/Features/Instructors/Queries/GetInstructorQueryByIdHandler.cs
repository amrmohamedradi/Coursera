using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Instructors.Queries
{
    public class GetInstructorQueryByIdHandler : IRequestHandler<GetInstructorQueryById,InstructorDto>
    {
        private readonly IApplicationDbContext _context;
        public GetInstructorQueryByIdHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<InstructorDto> Handle(GetInstructorQueryById request, CancellationToken cancellationToken)
        {
            var instructor = await _context.Instructors.AsNoTracking().FirstOrDefaultAsync(i => i.Id == request.Id,cancellationToken);
            if (instructor == null)
                throw new Exception("Instructor not found");
            return new InstructorDto(
                instructor.Id,
                instructor.Name,
                instructor.JobTitle,
                instructor.Bio,
                instructor.ImagePath);
        }
    }
}
