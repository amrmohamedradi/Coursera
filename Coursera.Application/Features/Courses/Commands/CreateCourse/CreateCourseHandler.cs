using Coursera.Application.Common.Interfaces;
using Coursera.Application.Features.Auth.Login;
using Coursera.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Courses.Commands.CreateCourse
{
    public class CreateCourseHandler : IRequestHandler<CreateCourseCommand,Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CreateCourseHandler> _logger;

        public CreateCourseHandler(IApplicationDbContext context, ILogger<CreateCourseHandler> logger)
        {
            _context = context;
            _logger = looger;
        }

        public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating course{CourseName}", request.Name);

            var course = new Course(request.Name, request.Description, request.Price, request.Level, request.ImagePath, request.CategoryId, request.InstructorId);
             _context.Courses.Add(course);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Course {CourseName} created successfully", request.Name);

            return course.Id;
        }
    }
}
