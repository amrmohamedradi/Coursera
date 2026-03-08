using Coursera.Application.Common.Exceptions;
using Coursera.Application.Common.Interfaces;
using Coursera.Application.Features.Courses.Commands.UpdateCourse;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Courses.Commands.DeleteCourse
{
    public class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<DeleteCourseHandler> _logger;

        public DeleteCourseHandler(IApplicationDbContext context, ILogger<DeleteCourseHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting course{CourseId}", request.Id);

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == request.Id,cancellationToken);
            if (course == null)
            {
                _logger.LogWarning("Course {CourseId} not found", request.Id);
                throw new NotFoundException("Course not found");
            }
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Course {CourseId} deleted successfully", request.Id);

            return Unit.Value;
        }
    }
}

