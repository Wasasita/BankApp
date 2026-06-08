using Xunit;
using BankingAPI.Services;
using BankingAPI.Models;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

[Collection("MongoDb collection")]
public class AuthServiceTests
{
    private readonly MongoDbFixture _fixture;

    public AuthServiceTests(MongoDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ShouldReturnToken()
    {
        var service = _fixture.AuthService;

        var request = new LoginRequest { Username = "admin", Password = "admin123" };
        var result = await service.LoginAsync(request);

        Assert.NotNull(result);
        Assert.NotEmpty(result!.Token);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_TokenShouldBeValid()
    {
        var service = _fixture.AuthService;

        var request = new LoginRequest { Username = "admin", Password = "admin123" };
        var result = await service.LoginAsync(request);

        var token = result!.Token;
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

        Assert.NotNull(jwtToken);
        Assert.Equal("admin", jwtToken!.Subject);
    }

    [Fact]
    public async Task LoginAsync_InvalidUsername_ShouldReturnNull()
    {
        var service = _fixture.AuthService;

        var request = new LoginRequest { Username = "nonexistent", Password = "password123" };
        var result = await service.LoginAsync(request);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ShouldReturnNull()
    {
        var service = _fixture.AuthService;

        var request = new LoginRequest { Username = "admin", Password = "wrongpassword" };
        var result = await service.LoginAsync(request);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_EmptyUsername_ShouldReturnNull()
    {
        var service = _fixture.AuthService;

        var request = new LoginRequest { Username = "", Password = "password123" };
        var result = await service.LoginAsync(request);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_EmptyPassword_ShouldReturnNull()
    {
        var service = _fixture.AuthService;

        var request = new LoginRequest { Username = "admin", Password = "" };
        var result = await service.LoginAsync(request);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_NullUsername_ShouldReturnNull()
    {
        var service = _fixture.AuthService;

        var request = new LoginRequest { Username = null!, Password = "password123" };
        var result = await service.LoginAsync(request);

        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_UserRole_ShouldReturnToken()
    {
        var service = _fixture.AuthService;

        var request = new LoginRequest { Username = "testuser", Password = "user123" };
        var result = await service.LoginAsync(request);

        Assert.NotNull(result);
        Assert.NotEmpty(result!.Token);
    }

    [Fact]
    public async Task LoginAsync_TokenShouldContainUsernameInClaims()
    {
        var service = _fixture.AuthService;

        var request = new LoginRequest { Username = "testuser", Password = "user123" };
        var result = await service.LoginAsync(request);

        var token = result!.Token;
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

        var usernameClaim = jwtToken!.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
        Assert.NotNull(usernameClaim);
        Assert.Equal("testuser", usernameClaim!.Value);
    }

    [Fact]
    public async Task LoginAsync_TokenShouldContainRoleInClaims()
    {
        var service = _fixture.AuthService;

        var request = new LoginRequest { Username = "admin", Password = "admin123" };
        var result = await service.LoginAsync(request);

        var token = result!.Token;
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

        var roleClaim = jwtToken!.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        Assert.NotNull(roleClaim);
        Assert.Equal("Admin", roleClaim!.Value);
    }
}
