using Coursera.Application.Common.DTOs;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Application.Interfaces
{
    public interface IJwtService
    {
        
        Task<string> GenerateTokenAsync(UserTokenDto user);
        string GenerateRefreshToken();
    }
}
