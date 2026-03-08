using Coursera.Application.Common.Exceptions;
using Coursera.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Carts.Commands.RemoveCart
{
    public class RemoveCartHandler : IRequestHandler<RemoveCartCommand>
    {
        private readonly IApplicationDbContext _context;

        public RemoveCartHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);
            if (cart == null)
                throw new NotFoundException("Cart not found");
            cart.RemoveItem(request.CourseId);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
