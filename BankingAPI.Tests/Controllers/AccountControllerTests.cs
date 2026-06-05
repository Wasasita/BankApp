using Xunit;
using BankingAPI.Controllers;
using BankingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Collection("MongoDb collection")]
public class AccountControllerTests
{
    private readonly MongoDbFixture _fixture;

    public AccountControllerTests(MongoDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAccountById_Invalid_ShouldReturnNotFound()
    {
        var controller = new AccountsController(_fixture.AccountService);

        var result = await controller.GetAccountById(9999);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateAccount_ValidCustomer_ShouldReturnCreated()
    {
        var controller = new AccountsController(_fixture.AccountService);

        var account = new Account
        {
            AccountNumber = "99900001",
            AccountType = "Savings",
            Balance = 250m
        };

        var result = await controller.CreateAccount(1, account);

        Assert.IsType<CreatedAtActionResult>(result.Result);
    }

    [Fact]
    public async Task SearchAccounts_ByCustomerName_ShouldReturnMatches()
    {
        var controller = new AccountsController(_fixture.AccountService);

        var result = await controller.SearchAccounts("alice", 1, 10);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var accounts = Assert.IsType<List<AccountResponse>>(okResult.Value);

        Assert.NotEmpty(accounts);
        Assert.All(accounts, account => Assert.Contains("Alice", account.CustomerName, System.StringComparison.OrdinalIgnoreCase));
    }
}
