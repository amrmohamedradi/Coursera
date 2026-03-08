using Coursera.Infrastructure.Identity;

namespace Coursera.Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(ApplicationUser user);
        string GenerateRefreshToken();
    }
}
