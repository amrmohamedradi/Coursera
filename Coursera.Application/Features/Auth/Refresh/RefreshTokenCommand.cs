using MediatR;

namespace Coursera.Application.Features.Auth.Refresh
{
    public class RefreshTokenCommand : IRequest<AuthResponse>
    {
        public string Email { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
    }
}
