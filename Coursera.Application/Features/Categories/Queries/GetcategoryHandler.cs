using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Categories.Queries
{
    public class GetcategoryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetcategoryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return  await _context.Categories.AsNoTracking().Select(c => new CategoryDto(c.Id, c.Name, c.ImagePath)).ToListAsync( cancellationToken);

        }
    }
}
