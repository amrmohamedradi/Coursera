using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Auth.Register
{
    public record RegisterCommand(string FirstName, string LastName, string Email, string Password) : IRequest<AuthResponse>;
    
}
