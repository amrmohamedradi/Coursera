using MediatR;

namespace Coursera.Application.Features.Auth.ExternalLogin
{
    /// <summary>
    /// Command issued when the client has already obtained an ID token (or access token)
    /// from an external provider SDK (Google Sign-In, Facebook Login) and wants the backend
    /// to validate it and return a JWT + refresh-token pair.
    /// </summary>
    /// <param name="Provider">Either "google" or "facebook" (case-insensitive).</param>
    /// <param name="IdToken">The ID token / access token returned by the provider's client SDK.</param>
    public record ExternalLoginCommand(string Provider, string IdToken) : IRequest<AuthResponse>;
}
