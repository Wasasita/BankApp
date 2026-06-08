using Xunit;
using BankingAPI.Models;
using System.Threading.Tasks;

[Collection("MongoDb collection")]
public class UserRepositoryTests
{
    private readonly MongoDbFixture _fixture;

    public UserRepositoryTests(MongoDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetByUsernameAsync_ValidUsername_ShouldReturnUser()
    {
        var repo = _fixture.UserRepository;

        var result = await repo.GetByUsernameAsync("admin");

        Assert.NotNull(result);
        Assert.Equal("admin", result!.Username);
    }

    [Fact]
    public async Task GetByUsernameAsync_ValidUsername_ShouldReturnAllUserDetails()
    {
        var repo = _fixture.UserRepository;

        var result = await repo.GetByUsernameAsync("admin");

        Assert.NotNull(result);
        Assert.Equal("admin", result!.Username);
        Assert.Equal("Admin", result.Role);
        Assert.NotEmpty(result.PasswordHash);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task GetByUsernameAsync_InvalidUsername_ShouldReturnNull()
    {
        var repo = _fixture.UserRepository;

        var result = await repo.GetByUsernameAsync("nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUsernameAsync_DifferentUsers_ShouldReturnCorrectUser()
    {
        var repo = _fixture.UserRepository;

        var adminResult = await repo.GetByUsernameAsync("admin");
        var userResult = await repo.GetByUsernameAsync("testuser");

        Assert.NotNull(adminResult);
        Assert.NotNull(userResult);
        Assert.Equal("Admin", adminResult!.Role);
        Assert.Equal("User", userResult!.Role);
    }

    [Fact]
    public async Task GetByUsernameAsync_CaseSensitive_ShouldNotFindWithDifferentCase()
    {
        var repo = _fixture.UserRepository;

        var result = await repo.GetByUsernameAsync("ADMIN");

        Assert.Null(result);
    }
}
