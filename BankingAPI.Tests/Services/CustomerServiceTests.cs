using Xunit;
using BankingAPI.Services;
using BankingAPI.Models;
using System.Linq;
using System.Threading.Tasks;

[Collection("MongoDb collection")]
public class CustomerServiceTests
{
    private readonly MongoDbFixture _fixture;

    public CustomerServiceTests(MongoDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CreateCustomer_ShouldAssignId()
    {
        var service = _fixture.CustomerService;

        var customer = new Customer
        {
            Name = "Test",
            Email = "test@test.com"
        };

        var result = await service.CreateCustomer(customer);

        var created = Assert.IsType<Customer>(result);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task GetCustomerById_ShouldReturnCustomer()
    {
        var service = _fixture.CustomerService;

        var result = await service.GetCustomerById(1);

        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetPremiumCustomers_ShouldReturnOnlyHighBalance()
    {
        var service = _fixture.CustomerService;

        var result = await service.GetAllPremiumCustomers(1000);

        Assert.All(result, c =>
            Assert.True(c.Accounts.Sum(a => a.Balance) > 1000));
    }

    [Fact]
    public async Task CreateAccount_ShouldAddAccount()
    {
        var service = _fixture.AccountService;

        var account = new Account
        {
            AccountNumber = "123",
            AccountType = "Savings",
            Balance = 100
        };

        var result = await service.CreateAccountAsync(1, account);

        Assert.NotNull(result);
        Assert.True(result!.Id > 0);
    }

    [Fact]
    public async Task GetCustomerById_InvalidId_ShouldReturnNull()
    {
        var service = _fixture.CustomerService;

        var result = await service.GetCustomerById(9999);

        Assert.Null(result);
    }
}