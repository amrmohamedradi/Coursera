using Coursera.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Courses.Commands.UpdateCourse
{
    public class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        public UpdateCourseHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Unit> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == request.id, cancellationToken);
            if (course == null)
                throw new Exception("Course not found");
            course.Update(request.Name,request.Description,request.Price,request.Level,request.ImagePath,request.CategoryId,request.InstructorId);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
