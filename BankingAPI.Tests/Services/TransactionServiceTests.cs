using Xunit;
using BankingAPI.Services;
using BankingAPI.Models;
using System.Linq;
using System.Threading.Tasks;

[Collection("MongoDb collection")]
public class TransactionServiceTests
{
    private readonly MongoDbFixture _fixture;

    public TransactionServiceTests(MongoDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DepositAsync_ValidDeposit_ShouldSucceed()
    {
        var service = _fixture.TransactionService;

        var result = await service.DepositAsync(1, 500m, "Test deposit");

        Assert.Equal(TransactionOperationResult.Success, result);
    }

    [Fact]
    public async Task DepositAsync_InvalidAmount_ShouldReturnInvalidAmount()
    {
        var service = _fixture.TransactionService;

        var result = await service.DepositAsync(1, -100m);

        Assert.Equal(TransactionOperationResult.InvalidAmount, result);
    }

    [Fact]
    public async Task DepositAsync_ZeroAmount_ShouldReturnInvalidAmount()
    {
        var service = _fixture.TransactionService;

        var result = await service.DepositAsync(1, 0m);

        Assert.Equal(TransactionOperationResult.InvalidAmount, result);
    }

    [Fact]
    public async Task DepositAsync_AccountNotFound_ShouldReturnAccountNotFound()
    {
        var service = _fixture.TransactionService;

        var result = await service.DepositAsync(9999, 100m);

        Assert.Equal(TransactionOperationResult.AccountNotFound, result);
    }

    [Fact]
    public async Task DepositAsync_ShouldUpdateAccountBalance()
    {
        var service = _fixture.TransactionService;
        var accountRepo = _fixture.AccountRepository;

        var initialAccount = await accountRepo.GetByIdAsync(1);
        var initialBalance = initialAccount!.Balance;

        await service.DepositAsync(1, 250m);

        var updatedAccount = await accountRepo.GetByIdAsync(1);
        Assert.Equal(initialBalance + 250m, updatedAccount!.Balance);
    }

    [Fact]
    public async Task DepositAsync_ShouldCreateTransaction()
    {
        var service = _fixture.TransactionService;
        var transactionRepo = _fixture.TransactionRepository;

        var initialCount = (await transactionRepo.GetAllAsync()).Count;

        await service.DepositAsync(1, 100m, "New deposit");

        var updatedCount = (await transactionRepo.GetAllAsync()).Count;
        Assert.Equal(initialCount + 1, updatedCount);

        var lastTransaction = (await transactionRepo.GetAllAsync()).OrderByDescending(t => t.Id).First();
        Assert.Equal("Deposit", lastTransaction.Type);
        Assert.Equal(100m, lastTransaction.Amount);
        Assert.Equal(1, lastTransaction.AccountId);
        Assert.Equal("New deposit", lastTransaction.Description);
    }

    [Fact]
    public async Task WithdrawAsync_ValidWithdraw_ShouldSucceed()
    {
        var service = _fixture.TransactionService;

        var result = await service.WithdrawAsync(1, 100m, "Test withdrawal");

        Assert.Equal(TransactionOperationResult.Success, result);
    }

    [Fact]
    public async Task WithdrawAsync_InvalidAmount_ShouldReturnInvalidAmount()
    {
        var service = _fixture.TransactionService;

        var result = await service.WithdrawAsync(1, -50m);

        Assert.Equal(TransactionOperationResult.InvalidAmount, result);
    }

    [Fact]
    public async Task WithdrawAsync_InsufficientFunds_ShouldReturnInsufficientFunds()
    {
        var service = _fixture.TransactionService;

        var result = await service.WithdrawAsync(1, 999999m);

        Assert.Equal(TransactionOperationResult.InsufficientFunds, result);
    }

    [Fact]
    public async Task WithdrawAsync_AccountNotFound_ShouldReturnAccountNotFound()
    {
        var service = _fixture.TransactionService;

        var result = await service.WithdrawAsync(9999, 100m);

        Assert.Equal(TransactionOperationResult.AccountNotFound, result);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldUpdateAccountBalance()
    {
        var service = _fixture.TransactionService;
        var accountRepo = _fixture.AccountRepository;

        var initialAccount = await accountRepo.GetByIdAsync(1);
        var initialBalance = initialAccount!.Balance;

        await service.WithdrawAsync(1, 100m);

        var updatedAccount = await accountRepo.GetByIdAsync(1);
        Assert.Equal(initialBalance - 100m, updatedAccount!.Balance);
    }

    [Fact]
    public async Task WithdrawAsync_ShouldCreateTransaction()
    {
        var service = _fixture.TransactionService;
        var transactionRepo = _fixture.TransactionRepository;

        var initialCount = (await transactionRepo.GetAllAsync()).Count;

        await service.WithdrawAsync(1, 50m, "New withdrawal");

        var updatedCount = (await transactionRepo.GetAllAsync()).Count;
        Assert.Equal(initialCount + 1, updatedCount);

        var lastTransaction = (await transactionRepo.GetAllAsync()).OrderByDescending(t => t.Id).First();
        Assert.Equal("Withdrawal", lastTransaction.Type);
        Assert.Equal(50m, lastTransaction.Amount);
        Assert.Equal(1, lastTransaction.AccountId);
    }

    [Fact]
    public async Task TransferAsync_ValidTransfer_ShouldSucceed()
    {
        var service = _fixture.TransactionService;

        var result = await service.TransferAsync(1, 2, 200m, "Test transfer");

        Assert.Equal(TransactionOperationResult.Success, result);
    }

    [Fact]
    public async Task TransferAsync_InvalidAmount_ShouldReturnInvalidAmount()
    {
        var service = _fixture.TransactionService;

        var result = await service.TransferAsync(1, 2, -100m);

        Assert.Equal(TransactionOperationResult.InvalidAmount, result);
    }

    [Fact]
    public async Task TransferAsync_SameAccountTransfer_ShouldReturnSameAccountTransfer()
    {
        var service = _fixture.TransactionService;

        var result = await service.TransferAsync(1, 1, 100m);

        Assert.Equal(TransactionOperationResult.SameAccountTransfer, result);
    }

    [Fact]
    public async Task TransferAsync_InsufficientFunds_ShouldReturnInsufficientFunds()
    {
        var service = _fixture.TransactionService;

        var result = await service.TransferAsync(1, 2, 999999m);

        Assert.Equal(TransactionOperationResult.InsufficientFunds, result);
    }

    [Fact]
    public async Task TransferAsync_FromAccountNotFound_ShouldReturnAccountNotFound()
    {
        var service = _fixture.TransactionService;

        var result = await service.TransferAsync(9999, 2, 100m);

        Assert.Equal(TransactionOperationResult.AccountNotFound, result);
    }

    [Fact]
    public async Task TransferAsync_ToAccountNotFound_ShouldReturnAccountNotFound()
    {
        var service = _fixture.TransactionService;

        var result = await service.TransferAsync(1, 9999, 100m);

        Assert.Equal(TransactionOperationResult.AccountNotFound, result);
    }

    [Fact]
    public async Task TransferAsync_ShouldUpdateBothBalances()
    {
        var service = _fixture.TransactionService;
        var accountRepo = _fixture.AccountRepository;

        var fromAccountBefore = await accountRepo.GetByIdAsync(1);
        var toAccountBefore = await accountRepo.GetByIdAsync(2);

        await service.TransferAsync(1, 2, 150m);

        var fromAccountAfter = await accountRepo.GetByIdAsync(1);
        var toAccountAfter = await accountRepo.GetByIdAsync(2);

        Assert.Equal(fromAccountBefore!.Balance - 150m, fromAccountAfter!.Balance);
        Assert.Equal(toAccountBefore!.Balance + 150m, toAccountAfter!.Balance);
    }

    [Fact]
    public async Task TransferAsync_ShouldCreateTwoTransactions()
    {
        var service = _fixture.TransactionService;
        var transactionRepo = _fixture.TransactionRepository;

        var initialCount = (await transactionRepo.GetAllAsync()).Count;

        await service.TransferAsync(1, 2, 100m, "Test transfer");

        var updatedCount = (await transactionRepo.GetAllAsync()).Count;
        Assert.Equal(initialCount + 2, updatedCount);

        var allTransactions = await transactionRepo.GetAllAsync();
        var fromTransactions = allTransactions.Where(t => t.AccountId == 1).ToList();
        var toTransactions = allTransactions.Where(t => t.AccountId == 2).ToList();

        var fromTransaction = fromTransactions.OrderByDescending(t => t.Id).First();
        var toTransaction = toTransactions.OrderByDescending(t => t.Id).First();

        Assert.Equal("Transfer", fromTransaction.Type);
        Assert.Equal(100m, fromTransaction.Amount);
        Assert.Equal("Transfer", toTransaction.Type);
        Assert.Equal(100m, toTransaction.Amount);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTransactions()
    {
        var service = _fixture.TransactionService;

        var result = await service.GetAllAsync();

        Assert.NotEmpty(result);
        Assert.True(result.Count > 0);
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ShouldReturnTransaction()
    {
        var service = _fixture.TransactionService;

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ShouldReturnNull()
    {
        var service = _fixture.TransactionService;

        var result = await service.GetByIdAsync(9999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByAccountIdAsync_ShouldReturnAccountTransactions()
    {
        var service = _fixture.TransactionService;

        var result = await service.GetByAccountIdAsync(1);

        Assert.NotEmpty(result);
        Assert.All(result, t => Assert.Equal(1, t.AccountId));
    }

    [Fact]
    public async Task GetByAccountIdAsync_NoTransactions_ShouldReturnEmpty()
    {
        var service = _fixture.TransactionService;

        var result = await service.GetByAccountIdAsync(9999);

        Assert.Empty(result);
    }
}
