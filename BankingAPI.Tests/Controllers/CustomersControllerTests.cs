using Xunit;
using BankingAPI.Controllers;
using BankingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using BankingAPI.Services;

public class CustomersControllerTests
{
    [Fact]
    public void GetCustomerById_Invalid_ShouldReturnNotFound()
    {
        var service = new CustomerService();
        var controller = new CustomersController(service);

        var result = controller.GetCustomerById(999);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }
}