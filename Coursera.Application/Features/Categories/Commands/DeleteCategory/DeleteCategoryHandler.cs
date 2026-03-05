using Coursera.Application.Common.Interfaces;
using Coursera.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Categories.Commands.DeleteCategory
{
    internal class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        public DeleteCategoryHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (category == null)
                throw new Exception("Category not found");
            var hasCourses = await _context.Courses.AnyAsync(c => c.CategoryId == request.Id, cancellationToken);
            if (hasCourses)
                throw new Exception("Cannot delete category with courses");
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
