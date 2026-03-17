using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Exceptions;
using Coursera.Application.Common.Interfaces;
using Coursera.Domain.Entities;
using Coursera.Infrastructure.Data;
using Coursera.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursera.Infrastructure.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public AuthService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        public async Task<UserTokenDto> RegisterAsync(string firstName, string lastName, string email, string password)
        {
            var user = new ApplicationUser(firstName, lastName, email, email);
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new ValidationException(string.Join(",", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "User");
            var roles = (await _userManager.GetRolesAsync(user)).ToList();

            return new UserTokenDto
            {
                Id = user.Id,
                Email = user.Email!,
                Roles = roles
            };
        }
        public async Task<UserTokenDto> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                throw new UnauthorizedException("Invalid email or password.");
            var roles = (await _userManager.GetRolesAsync(user)).ToList();

            return new UserTokenDto
            {
                Id = user.Id,
                Email = user.Email!,
                Roles = roles
            };
        }

        public async Task SetRefreshTokenAsync(Guid userId, string refreshToken, DateTime refreshTokenExpiryTime)
        {
            var user = await _context.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                throw new NotFoundException("User not found.");

            var token = new RefreshToken
            {
                Token = refreshToken,
                ExpiryDate = refreshTokenExpiryTime,
                IsRevoked = false,
                UserId = userId,
                ApplicationUserId = userId
            };

            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<UserTokenDto> RefreshTokenAsync(string email, string refreshToken)
        {
            var user = await _userManager.Users.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                throw new UnauthorizedException("Invalid refresh token.");

            var activeToken = user.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);
            
            if (activeToken == null || !activeToken.IsActive)
                throw new UnauthorizedException("Invalid or expired refresh token.");

            activeToken.IsRevoked = true;
            await _context.SaveChangesAsync();

            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            return new UserTokenDto
            {
                Id = user.Id,
                Email = user.Email!,
                Roles = roles
            };
        }
    }
}
