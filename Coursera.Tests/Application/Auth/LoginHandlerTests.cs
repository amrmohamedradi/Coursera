using Coursera.Application.Common.DTOs;
using Coursera.Application.Features.Auth.Login;
using Coursera.Application.Interfaces;
using Coursera.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Coursera.Application.Common.Models;
using Coursera.Application.Common.Interfaces;

namespace Coursera.Tests.Application.Auth
{
    public class LoginHandlerTests
    {
        private readonly Mock<IAuthService> _authServiceMock = new();
        private readonly Mock<IJwtService> _jwtServiceMock = new();
        private readonly Mock<ILogger<LoginHandler>> _loggerMock = new();



        [Fact]
        public async Task Login_Should_Return_Token_When_Credentials_Are_Correct()
        {
            var email = "test@test.com";
            var password = "Password123!";
            _authServiceMock
                .Setup(x => x.LoginAsync(email, password))
                .ReturnsAsync(new UserTokenDto
                {
                    Id = Guid.NewGuid(),
                    Email = email

                });
            var user = new ApplicationUser("Test", "User", "Test", email);
            var command = new LoginCommand(email, password);
            _jwtServiceMock.Setup(x => x.GenerateTokenAsync(It.IsAny<UserTokenDto>())).ReturnsAsync("fake-jwt-token");
            var jwtSettings = Options.Create(new JwtSettings());
            var handler = new LoginHandler(_authServiceMock.Object, _jwtServiceMock.Object, jwtSettings,_loggerMock.Object);
            var result = await handler.Handle(command, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal("fake-jwt-token", result.Token);
        }
    }
}
