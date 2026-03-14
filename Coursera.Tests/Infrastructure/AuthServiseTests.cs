using Coursera.Application.Common.DTOs;
using Coursera.Application.Common.Exceptions;
using Coursera.Application.Interfaces;
using Coursera.Infrastructure.Identity;
using Coursera.Infrastructure.Service;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Threading.Tasks;
using Xunit;
using MockQueryable.Moq;
using System.Linq;
using System.Collections.Generic;

namespace Coursera.Tests.Infrastructure;
public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

    public AuthServiceTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();

        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object,
            null, null, null, null, null, null, null, null);
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
        var service = new AuthService(_userManagerMock.Object);
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
        var service = new AuthService(_userManagerMock.Object);
        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            service.LoginAsync(email, password));
    }
    [Fact]
    public async Task RefreshTokenAsync_Should_Return_New_Token_When_Token_Is_Valid()
    {
        var user = new ApplicationUser("Test", "User", "Test", "test@test.com");
        var users = new List<ApplicationUser> { user }.AsQueryable().BuildMock();
        
        _userManagerMock.Setup(x => x.Users)
            .Returns(users);
        _userManagerMock.Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);
        var refreshToken = "valid-refresh-token";
        var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        
        var service = new AuthService(
            _userManagerMock.Object
        );
        await service.SetRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiryTime);
        Assert.Single(user.RefreshTokens);
        Assert.Equal("valid-refresh-token", user.RefreshTokens.First().Token);
        Assert.Equal(refreshTokenExpiryTime, user.RefreshTokens.First().ExpiryDate);
    }
}