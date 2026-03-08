using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Exceptions;
using Coursera.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Categories.Queries
{
    public class GetcategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery,CategoryDto>
    {
        private readonly IApplicationDbContext _context;
        public GetcategoryByIdHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        

        public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (category == null)
                throw new NotFoundException("Category not found");
            return new CategoryDto(
                category.Id, category.Name, category.ImagePath);
        }
    }
}
