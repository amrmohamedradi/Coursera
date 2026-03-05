using Coursera.Application.Common.Interfaces;
using Coursera.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Instructors.Commands.CreateInstructor
{
    public class CreateInstructorHandler : IRequestHandler<CreateInstructorCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        public CreateInstructorHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> Handle(CreateInstructorCommand request, CancellationToken cancellationToken)
        {

            var instructor = new Instructor(request.Name, request.Bio, request.JobTitle,request.ImgagePath);
            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync(cancellationToken);
            return instructor.Id;
        }
    }
}
