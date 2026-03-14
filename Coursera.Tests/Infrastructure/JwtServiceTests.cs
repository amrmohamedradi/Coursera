using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Models;
using Coursera.Infrastructure.Service;
using Microsoft.Extensions.Options;

namespace Coursera.Tests.Infrastructure;

public class JwtServiceTests
{
    [Fact]
    public async Task GenerateToken_Should_Return_String_Token()
    {
        var settings = Options.Create(new JwtSettings
        {
            Key = "THIS_IS_A_SUPER_SECRET_KEY_123456789",
            Issuer = "CourseraApi",
            Audience = "CourseraClient",
            DurationInHours = 2
        });
        var service = new JwtService(settings);
        var user = new UserTokenDto
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com"
        };
        var token = await service.GenerateTokenAsync(user);
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }
}