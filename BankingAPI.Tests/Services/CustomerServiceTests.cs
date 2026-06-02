using Xunit;
using BankingAPI.Services;
using BankingAPI.Models;

public class CustomerServiceTests
{
    [Fact]
    public void CreateCustomer_ShouldAssignId()
    {
        var service = new CustomerService();

        var customer = new Customer
        {
            Name = "Test",
            Email = "test@test.com"
        };

        var result = service.CreateCustomer(customer);

        var created = Assert.IsType<Customer>(result);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public void GetCustomerById_ShouldReturnCustomer()
    {
        var service = new CustomerService();

        var result = service.GetCustomerById(1);

        Assert.NotNull(result);
    }

    [Fact]
    public void GetPremiumCustomers_ShouldReturnOnlyHighBalance()
    {
        var service = new CustomerService();

        var result = service.GetAllPremiumCustomers(1000);

        Assert.All(result, c =>
            Assert.True(c.Accounts.Sum(a => a.Balance) > 1000));
    }

    [Fact]
    public void CreateAccount_ShouldAddAccount()
    {
        var service = new AccountService();

        var account = new Account
        {
            AccountNumber = "123",
            AccountType = "Savings",
            Balance = 100
        };

        var result = service.CreateAccount(1, account);

        Assert.NotNull(result);
        Assert.True(result!.Id > 0);
    }

    [Fact]
    public void GetCustomerById_InvalidId_ShouldReturnNull()
    {
        var service = new CustomerService();

        var result = service.GetCustomerById(9999);

        Assert.Null(result);
    }
}