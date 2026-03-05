using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Interfaces;
using Coursera.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
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
        public AuthService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }


        public async Task<UserTokenDto> RegisterAsync(string firstName, string lastName, string email, string password)
        {
            var user = new ApplicationUser(firstName, lastName, email, email);
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "User");
            var roles = (await _userManager.GetRolesAsync(user)).ToList();

            return new UserTokenDto
            {
                Id = user.Id,
                Email = user.Email,
                Roles = roles
            };
        }
        public async Task<UserTokenDto> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                throw new Exception("Invalid email or password.");
            var roles = (await _userManager.GetRolesAsync(user)).ToList();

            return new UserTokenDto
            {
                Id = user.Id,
                Email = user.Email,
                Roles = roles
            };
        }
    }
}
