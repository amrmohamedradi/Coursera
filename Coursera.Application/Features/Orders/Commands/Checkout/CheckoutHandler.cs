using Coursera.Application.Common.Exceptions;
using Coursera.Application.Common.Interfaces;
using Coursera.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Orders.Commands.Checkout
{
    public class CheckoutHandler : IRequestHandler<CheckoutCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CheckoutHandler> _logger;

        public CheckoutHandler(IApplicationDbContext context, ILogger<CheckoutHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Guid> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User{UserId} starting checkout", request.UserId);
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);
            if (cart == null || !cart.Items.Any())
            {
                _logger.LogWarning("Checkout failed for user {UserId} because cart is empty", request.UserId);
                throw new NotFoundException("Cart is empty");

            }
            var order = new Order(request.UserId, cart.Items.ToList());
            _context.Orders.Add(order);
            cart.Clear();
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Checkout complated successfully for user {UserId} with order {OrderId}", request.UserId,order.Id);

            return order.Id;
            
        }
    }
}
