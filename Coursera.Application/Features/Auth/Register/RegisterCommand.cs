using Coursera.Application.Features.Auth;
using MediatR;

namespace Coursera.Application.Features.Auth.Register
{
    public class RegisterCommand : IRequest<AuthResponse>
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
