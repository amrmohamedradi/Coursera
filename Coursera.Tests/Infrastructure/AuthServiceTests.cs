using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Exceptions;
using Coursera.Application.Interfaces;
using Coursera.Domain.Entities;
using Coursera.Infrastructure.Data;
using Coursera.Infrastructure.Identity;
using Coursera.Infrastructure.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MockQueryable.Moq;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Coursera.Tests.Infrastructure;
public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly ApplicationDbContext _context;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly ILogger<AuthService> _logger = NullLogger<AuthService>.Instance;

    public AuthServiceTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();

        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        // Create a fresh in-memory database for each test class instance.
        // This is the same approach used throughout the test suite and avoids
        // needing a real SQL Server connection or xUnit fixture wiring.
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        _configurationMock = new Mock<IConfiguration>();
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
    }

    [Fact]
    public async Task LoginAsync_Should_Return_User_When_Credentials_Are_Correct()
    {
        var email = "test@test.com";
        var password = "Password123!";
        var user = new ApplicationUser("Test", "User", "Test", email);
        _userManagerMock
            .Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync(user);
        _userManagerMock
            .Setup(x => x.CheckPasswordAsync(user, password))
            .ReturnsAsync(true);
        _userManagerMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });
        var service = new AuthService(_userManagerMock.Object, _context, _configurationMock.Object, _httpClientFactoryMock.Object, _logger);
        var result = await service.LoginAsync(email, password);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task LoginAsync_Should_Throw_When_User_Not_Found()
    {
        var email = "test@test.com";
        var password = "Password123!";
        _userManagerMock
            .Setup(x => x.FindByEmailAsync(email))
            .ReturnsAsync((ApplicationUser?)null);
        var service = new AuthService(_userManagerMock.Object, _context, _configurationMock.Object, _httpClientFactoryMock.Object, _logger);
        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            service.LoginAsync(email, password));
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_Return_New_Token_When_Token_Is_Valid()
    {
        var user = new ApplicationUser("Test", "User", "Test", "test@test.com");

        // SetRefreshTokenAsync queries _context.Users directly, so the user
        // must exist in the in-memory database — not just in the UserManager mock.
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _userManagerMock.Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);
        var refreshToken = "valid-refresh-token";
        var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        var service = new AuthService(
            _userManagerMock.Object,
            _context,
            _configurationMock.Object,
            _httpClientFactoryMock.Object,
            _logger
        );
        await service.SetRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiryTime);

        // Reload the user from context: SetRefreshTokenAsync adds the token via
        // _context.RefreshTokens.AddAsync, not user.RefreshTokens.Add,
        // so the in-memory user object is stale until re-queried.
        var updatedUser = await _context.Users
            .Include(u => u.RefreshTokens)
            .FirstAsync(u => u.Id == user.Id);

        Assert.Single(updatedUser.RefreshTokens);
        Assert.Equal("valid-refresh-token", updatedUser.RefreshTokens.First().Token);
        Assert.Equal(refreshTokenExpiryTime, updatedUser.RefreshTokens.First().ExpiryDate);
    }
}