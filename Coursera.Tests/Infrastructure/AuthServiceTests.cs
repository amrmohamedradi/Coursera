using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Exceptions;
using Coursera.Application.Interfaces;
using Coursera.Domain.Entities;
using Coursera.Infrastructure.Data;
using Coursera.Infrastructure.Identity;
using Coursera.Infrastructure.Service;
using Microsoft.AspNetCore.Identity;
using MockQueryable.Moq;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Coursera.Tests.Infrastructure;
public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly ApplicationDbContext _context;

    public AuthServiceTests(ApplicationDbContext context)
    {
        var store = new Mock<IUserStore<ApplicationUser>>();

        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null, null, null, null, null, null, null, null);
        _context = context;
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
        var service = new AuthService(_userManagerMock.Object,_context);
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
            .ReturnsAsync((ApplicationUser)null);
        var service = new AuthService(_userManagerMock.Object, _context);
        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            service.LoginAsync(email, password));
    }
    [Fact]
    public async Task RefreshTokenAsync_Should_Return_New_Token_When_Token_Is_Valid()
    {
        var user = new ApplicationUser("Test", "User", "Test", "test@test.com");
        var usersMock = new List<ApplicationUser> { user }.BuildMockDbSet();
        
        _userManagerMock.Setup(x => x.Users)
            .Returns(usersMock.Object);
        _userManagerMock.Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);
        var refreshToken = "valid-refresh-token";
        var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        
        var service = new AuthService(
            _userManagerMock.Object,
            _context
        );
        await service.SetRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiryTime);
        Assert.Single(user.RefreshTokens);
        Assert.Equal("valid-refresh-token", user.RefreshTokens.First().Token);
        Assert.Equal(refreshTokenExpiryTime, user.RefreshTokens.First().ExpiryDate);
    }
}