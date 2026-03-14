using MediatR;

namespace Coursera.Application.Features.Auth.Refresh
{
    public record RefreshTokenCommand(string Email, string RefreshToken) : IRequest<AuthResponse>;
    
}
