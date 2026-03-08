using Coursera.Application.Common.Exceptions;
using Coursera.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Instructors.Commands.DeleteInstructor
{
    public class DeleteInstructorHandler : IRequestHandler<DeleteInstructorCommand,Unit>
    {
        private readonly IApplicationDbContext _context;
        public DeleteInstructorHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteInstructorCommand request, CancellationToken cancellationToken)
        {
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
            if (instructor == null)
                throw new NotFoundException("Instructor not found");
            var hasCourses = await _context.Courses.AnyAsync(c => c.InstructorId == request.Id, cancellationToken);
            if (hasCourses)
                throw new ValidationException("Cannot delete instructor with courses");
            _context.Instructors.Remove(instructor);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
