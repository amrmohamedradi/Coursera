using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Carts.Commands.RemoveCart
{
    public record RemoveCartCommand(Guid UserId,Guid CourseId) : IRequest;    
}
