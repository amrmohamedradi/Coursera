using Coursera.Application.Common.Exceptions;
using Coursera.Application.Common.Interfaces;
using Coursera.Application.Features.Courses.Commands.UpdateCourse;
using Coursera.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Carts.Commands.AddToCart
{
    public class AddToCartHandler : IRequestHandler<AddToCartCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<AddToCartHandler> _logger;


        public AddToCartHandler(IApplicationDbContext context, ILogger<AddToCartHandler> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User {UserId} adding course {CourseId} to cart", request.UserId,request.CourseId);

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId,cancellationToken);
            if (cart == null)
            {
                cart = new Cart(request.UserId);
                _context.Carts.Add(cart);
            }
            var course = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(i => i.Id == request.CourseId,cancellationToken);
            if(course == null)
            {
                throw new NotFoundException("Course not found"); 
            }
                cart.AddItem(course.Id, course.Price);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Course {CourseId} added to cart for user {UserId}",request.CourseId,request.UserId);
        }
    }
}
