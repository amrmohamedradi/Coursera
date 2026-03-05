using Coursera.Application.Common.Interfaces;
using Coursera.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Courses.Commands.CreateCourse
{
    public class CreateCourseHandler : IRequestHandler<CreateCourseCommand,Guid>
    {
        private readonly IApplicationDbContext _context;
        public CreateCourseHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = new Course(request.Name, request.Description, request.Price, request.Level, request.ImagePath, request.CategoryId, request.InstructorId);
             _context.Courses.Add(course);
            await _context.SaveChangesAsync(cancellationToken);
            return course.Id;
        }
    }
}
