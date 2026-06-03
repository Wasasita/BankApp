using Xunit;
using BankingAPI.Controllers;
using BankingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using BankingAPI.Services;

public class AccountControllerTests
{
	[Fact]
	public void GetAccountById_Invalid_ShouldReturnNotFound()
	{
		var service = new AccountService();
		var controller = new AccountsController(service);

		var result = controller.GetAccountById(9999);

		Assert.IsType<NotFoundObjectResult>(result.Result);
	}

	[Fact]
	public void CreateAccount_ValidCustomer_ShouldReturnCreated()
	{
		var service = new AccountService();
		var controller = new AccountsController(service);

		var account = new Account
		{
			AccountNumber = "99900001",
			AccountType = "Savings",
			Balance = 250m
		};

		var result = controller.CreateAccount(1, account);

		Assert.IsType<CreatedAtActionResult>(result.Result);
	}
}