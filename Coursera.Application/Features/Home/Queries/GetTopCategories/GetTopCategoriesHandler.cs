using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Home.Queries.GetTopCategories
{
    public class GetTopCategoriesHandler : IRequestHandler<GetTopCategoriesQuery,List<CategoryDto>>
    {
        private readonly IApplicationDbContext _context;
        public GetTopCategoriesHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> Handle(GetTopCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Categories
                .Take(6)
                .Select(c => new CategoryDto(c.Id, c.Name, c.ImagePath)).ToListAsync(cancellationToken);
        }
    }
}
