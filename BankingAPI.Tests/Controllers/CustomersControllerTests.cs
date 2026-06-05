using Xunit;
using BankingAPI.Controllers;
using BankingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Collection("MongoDb collection")]
public class CustomersControllerTests
{
    private readonly MongoDbFixture _fixture;

    public CustomersControllerTests(MongoDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetCustomerById_Invalid_ShouldReturnNotFound()
    {
        var controller = new CustomersController(_fixture.CustomerService);

        var result = await controller.GetCustomerById(999);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}