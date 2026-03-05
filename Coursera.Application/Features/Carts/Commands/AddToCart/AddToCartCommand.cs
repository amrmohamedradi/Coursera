using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Carts.Commands.AddToCart
{
    public record AddToCartCommand(Guid CourseId, Guid UserId) : IRequest;
    
}
