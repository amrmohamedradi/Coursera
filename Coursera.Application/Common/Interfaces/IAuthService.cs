using Coursera.Application.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Common.Interfaces
{
    public interface IAuthService
    {
        Task<UserTokenDto> RegisterAsync(
            string firstName,
            string lastName,
            string email,
            string password);
        Task<UserTokenDto> LoginAsync(
            string email,
            string password);

        Task SetRefreshTokenAsync(Guid userId, string refreshToken, DateTime refreshTokenExpiryTime);
        Task<UserTokenDto> RefreshTokenAsync(string email, string refreshToken);

        /// <summary>
        /// Validates an ID token / access token issued by an external provider
        /// (currently "google" or "facebook"), finds or creates the local user account,
        /// and returns a <see cref="UserTokenDto"/> ready for JWT issuance.
        /// </summary>
        Task<UserTokenDto> ExternalLoginAsync(string provider, string idToken);
    }
}
