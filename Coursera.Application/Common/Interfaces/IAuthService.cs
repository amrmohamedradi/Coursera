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
    }
}
