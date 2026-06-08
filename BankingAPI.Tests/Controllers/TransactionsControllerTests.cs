using Xunit;
using BankingAPI.Controllers;
using BankingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

[Collection("MongoDb collection")]
public class TransactionsControllerTests
{
    private readonly MongoDbFixture _fixture;

    public TransactionsControllerTests(MongoDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAllTransactions_ShouldReturnOkWithTransactions()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var result = await controller.GetAllTransactions();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var transactions = Assert.IsType<List<Transaction>>(okResult.Value);
        Assert.NotEmpty(transactions);
    }

    [Fact]
    public async Task GetTransactionById_ValidId_ShouldReturnOkWithTransaction()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var result = await controller.GetTransactionById(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var transaction = Assert.IsType<Transaction>(okResult.Value);
        Assert.Equal(1, transaction.Id);
    }

    [Fact]
    public async Task GetTransactionById_InvalidId_ShouldReturnNotFound()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var result = await controller.GetTransactionById(9999);

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetByAccountId_ValidAccountId_ShouldReturnTransactions()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var result = await controller.GetByAccountId(1);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var transactions = Assert.IsType<List<Transaction>>(okResult.Value);
        Assert.NotEmpty(transactions);
        Assert.All(transactions, t => Assert.Equal(1, t.AccountId));
    }

    [Fact]
    public async Task GetByAccountId_NoTransactions_ShouldReturnEmptyList()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var result = await controller.GetByAccountId(9999);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var transactions = Assert.IsType<List<Transaction>>(okResult.Value);
        Assert.Empty(transactions);
    }

    [Fact]
    public async Task Deposit_ValidRequest_ShouldReturnOk()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var request = new DepositRequest { AccountId = 1, Amount = 500m, Description = "Test deposit" };
        var result = await controller.Deposit(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Deposit_InvalidAmount_ShouldReturnBadRequest()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var request = new DepositRequest { AccountId = 1, Amount = -100m };
        var result = await controller.Deposit(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Deposit_AccountNotFound_ShouldReturnNotFound()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var request = new DepositRequest { AccountId = 9999, Amount = 100m };
        var result = await controller.Deposit(request);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Withdraw_ValidRequest_ShouldReturnOk()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var request = new WithdrawRequest { AccountId = 1, Amount = 100m };
        var result = await controller.Withdraw(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Withdraw_InvalidAmount_ShouldReturnBadRequest()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var request = new WithdrawRequest { AccountId = 1, Amount = -50m };
        var result = await controller.Withdraw(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Withdraw_InsufficientFunds_ShouldReturnBadRequest()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var request = new WithdrawRequest { AccountId = 1, Amount = 999999m };
        var result = await controller.Withdraw(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Withdraw_AccountNotFound_ShouldReturnNotFound()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var request = new WithdrawRequest { AccountId = 9999, Amount = 100m };
        var result = await controller.Withdraw(request);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Transfer_ValidRequest_ShouldReturnOk()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var request = new TransferRequest { FromAccountId = 1, ToAccountId = 2, Amount = 100m };
        var result = await controller.Transfer(request);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Transfer_InvalidAmount_ShouldReturnBadRequest()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var request = new TransferRequest { FromAccountId = 1, ToAccountId = 2, Amount = -50m };
        var result = await controller.Transfer(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Transfer_SameAccount_ShouldReturnBadRequest()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var request = new TransferRequest { FromAccountId = 1, ToAccountId = 1, Amount = 100m };
        var result = await controller.Transfer(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Transfer_InsufficientFunds_ShouldReturnBadRequest()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var request = new TransferRequest { FromAccountId = 1, ToAccountId = 2, Amount = 999999m };
        var result = await controller.Transfer(request);

        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Transfer_FromAccountNotFound_ShouldReturnNotFound()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var request = new TransferRequest { FromAccountId = 9999, ToAccountId = 2, Amount = 100m };
        var result = await controller.Transfer(request);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Transfer_ToAccountNotFound_ShouldReturnNotFound()
    {
        var controller = new TransactionsController(_fixture.TransactionService);

        var request = new TransferRequest { FromAccountId = 1, ToAccountId = 9999, Amount = 100m };
        var result = await controller.Transfer(request);

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
