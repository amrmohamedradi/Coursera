using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace Coursera.Application.Features.Carts.Queries.GetCart
{
    public class GetCartHandler : IRequestHandler<GetCartQuery,CartDto>
    {
        private readonly IApplicationDbContext _context;

        public GetCartHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var cart = await _context.Carts
                .Include( c=> c.Items)
                .ThenInclude(i => i.Course)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId,cancellationToken);
            if (cart == null)
                return new CartDto();
            var courses = cart.Items.Select(i => new CartItemDto
            {
                CourseId = i.CourseId,
                Description = i.Course.Description,
                Price = i.Price,
                ImagePath = i.Course.ImagePath
            }).ToList();
            var subtotal = courses.Sum(c => c.Price);
            var tax = subtotal * 0.15m;
            var total = subtotal + tax;
            return new CartDto
            {
                Courses = courses,
                Subtotal = subtotal,
                Tax = tax,
                Total = total
            };
        }
    }
}
