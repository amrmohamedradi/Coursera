using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Interfaces;
using Coursera.Application.Common.Models;
using Coursera.Application.Features.Auth.Register;
using Coursera.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coursera.Tests.Application.Auth
{
    public class RegisterHandlerTests
    {
        private readonly Mock<IAuthService> _authServiceMock = new();
        private readonly Mock<IJwtService> _jwtServiceMock = new();
        private readonly Mock<ILogger<RegisterHandler>> _loggerMock = new();

        [Fact]
        public async Task Register_Should_Return_Token_When_Registration_Is_Successful()
        {
            var email = "test@test.com";
            var password = "Password123!";
            var command = new RegisterCommand("Test", "User", email, password);
            var userToken = new UserTokenDto { Id = Guid.NewGuid(), Email = email, };
            _authServiceMock.Setup(s => s.RegisterAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(userToken);
            _jwtServiceMock.Setup(s => s.GenerateTokenAsync(It.IsAny<UserTokenDto>())).ReturnsAsync("fake-jwt-token");
            var jwtSetting = Options.Create(new JwtSettings());
            var handler = new RegisterHandler(_authServiceMock.Object, _jwtServiceMock.Object, jwtSetting, _loggerMock.Object);
            var result = await handler.Handle(command, CancellationToken.None);
            Assert.NotNull(result);
            Assert.Equal("fake-jwt-token", result.Token);
        }
    }
}
