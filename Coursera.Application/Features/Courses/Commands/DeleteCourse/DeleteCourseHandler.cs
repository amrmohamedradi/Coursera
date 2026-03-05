using Coursera.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Courses.Commands.DeleteCourse
{
    public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        public DeleteCourseHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == request.Id,cancellationToken);
            if (course == null)
                throw new Exception("Course not found");
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}

