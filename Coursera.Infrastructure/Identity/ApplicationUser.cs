using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FirstName { get;private set; }
        public string LastName { get;private set; }
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
