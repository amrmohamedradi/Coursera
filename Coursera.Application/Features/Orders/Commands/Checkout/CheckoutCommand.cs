using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Orders.Commands.Checkout
{
    public record CheckoutCommand(Guid UserId) : IRequest<Guid>;
}
