using Coursera.Application.Common.Exceptions;
using Coursera.Application.Common.Interfaces;
using Coursera.Application.Features.Auth.Login;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Courses.Commands.UpdateCourse
{
    public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<UpdateCourseHandler> _logger;

        public UpdateCourseHandler(IApplicationDbContext context, ILogger<UpdateCourseHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Unit> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating course{CourseName}", request.Name);

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == request.id, cancellationToken);
            if (course == null)
                throw new NotFoundException("Course not found");

            var isPurchased = await _context.OrderItems.AnyAsync(oi => oi.CourseId == request.id, cancellationToken);
            if (isPurchased)
                throw new ValidationException("Cannot update course because it was purchased");

            course.Update(request.Name,request.Description,request.Price,request.Level,request.ImagePath,request.CategoryId,request.InstructorId);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Course {CourseName} updated successfully", request.Name);

            return Unit.Value;
        }
    }
}
