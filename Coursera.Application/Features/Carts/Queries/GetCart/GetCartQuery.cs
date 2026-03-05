using Coursera.Application.Common.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Carts.Queries.GetCart
{
    public record GetCartQuery(Guid UserId) : IRequest<CartDto>;
}
