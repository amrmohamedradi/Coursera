using Coursera.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Instructors.Commands.UpdateInstructor
{
    public class UpdateInstructorHandler :IRequestHandler<UpdateInstructorCommand,Unit>
    {
        private readonly IApplicationDbContext _context;
        public UpdateInstructorHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateInstructorCommand request, CancellationToken cancellationToken)
        {
            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
            if (instructor == null)
                throw new Exception("Instructor not found");
            instructor.Update(request.Name, request.Bio, request.JobTitle, request.ImgagePath);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
