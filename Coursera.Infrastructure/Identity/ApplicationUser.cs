using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using Coursera.Domain.Entities;

namespace Coursera.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get;private set; } = default!;
        public string LastName { get;private set; } = default!;
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        private ApplicationUser() { }
        public ApplicationUser(string firstName, string lastName, string userName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            Email = email;
        }
        public void Update(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

    }
}
