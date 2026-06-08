using Xunit;
using BankingAPI.Controllers;
using BankingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Collection("MongoDb collection")]
public class AuthControllerTests
{
    private readonly MongoDbFixture _fixture;

    public AuthControllerTests(MongoDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Login_ValidCredentials_ShouldReturnOkWithToken()
    {
        var controller = new AuthController(_fixture.AuthService);

        var request = new LoginRequest { Username = "admin", Password = "admin123" };
        var result = await controller.Login(request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.NotEmpty(response.Token);
    }

    [Fact]
    public async Task Login_InvalidUsername_ShouldReturnUnauthorized()
    {
        var controller = new AuthController(_fixture.AuthService);

        var request = new LoginRequest { Username = "nonexistent", Password = "password123" };
        var result = await controller.Login(request);

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_InvalidPassword_ShouldReturnUnauthorized()
    {
        var controller = new AuthController(_fixture.AuthService);

        var request = new LoginRequest { Username = "admin", Password = "wrongpassword" };
        var result = await controller.Login(request);

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_EmptyUsername_ShouldReturnUnauthorized()
    {
        var controller = new AuthController(_fixture.AuthService);

        var request = new LoginRequest { Username = "", Password = "admin123" };
        var result = await controller.Login(request);

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_EmptyPassword_ShouldReturnUnauthorized()
    {
        var controller = new AuthController(_fixture.AuthService);

        var request = new LoginRequest { Username = "admin", Password = "" };
        var result = await controller.Login(request);

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_UserRole_ShouldReturnOkWithToken()
    {
        var controller = new AuthController(_fixture.AuthService);

        var request = new LoginRequest { Username = "testuser", Password = "user123" };
        var result = await controller.Login(request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<LoginResponse>(okResult.Value);
        Assert.NotEmpty(response.Token);
    }
}
