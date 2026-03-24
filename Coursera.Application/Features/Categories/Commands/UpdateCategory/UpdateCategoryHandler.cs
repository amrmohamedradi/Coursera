using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Exceptions;
using Coursera.Application.Common.Interfaces;
using Coursera.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand,Unit>
    {
        private readonly IApplicationDbContext _context;
        public UpdateCategoryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
            if (category == null)
                throw new NotFoundException("category not found");
            category.Update(request.Name, request.ImagePath);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
