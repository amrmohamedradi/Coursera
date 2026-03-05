using Coursera.Application.Common.Interfaces;
using Coursera.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Carts.Commands.AddToCart
{
    public class AddToCartHandler : IRequestHandler<AddToCartCommand>
    {
        private readonly IApplicationDbContext _context;

        public AddToCartHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId,cancellationToken);
            if (cart == null)
            {
                cart = new Cart(request.UserId);
                _context.Carts.Add(cart);
            }
            var course = await _context.Courses.FirstOrDefaultAsync(i => i.Id == request.CourseId,cancellationToken);
            if(course == null)
            {
                cart.AddItem(course.Id, course.Price);
                
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
