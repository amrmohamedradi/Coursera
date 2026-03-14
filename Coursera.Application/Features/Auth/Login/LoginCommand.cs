using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Features.Auth.Login
{
    public record LoginCommand(string Email,string Password) : IRequest<AuthResponse>;
    
       
}
