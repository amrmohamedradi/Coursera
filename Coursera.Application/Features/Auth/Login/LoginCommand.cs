using Coursera.Application.Features.Auth;
using MediatR;

namespace Coursera.Application.Features.Auth.Login
{
    public class LoginCommand : IRequest<AuthResponse>
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
