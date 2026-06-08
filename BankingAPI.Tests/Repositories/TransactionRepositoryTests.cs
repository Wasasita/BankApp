using Xunit;
using BankingAPI.Models;
using System.Threading.Tasks;
using System.Linq;

[Collection("MongoDb collection")]
public class TransactionRepositoryTests
{
    private readonly MongoDbFixture _fixture;

    public TransactionRepositoryTests(MongoDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTransactions()
    {
        var repo = _fixture.TransactionRepository;

        var result = await repo.GetAllAsync();

        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnSortedByCreatedAtDescending()
    {
        var repo = _fixture.TransactionRepository;

        var result = await repo.GetAllAsync();

        for (int i = 0; i < result.Count - 1; i++)
        {
            Assert.True(result[i].CreatedAt >= result[i + 1].CreatedAt);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ValidId_ShouldReturnTransaction()
    {
        var repo = _fixture.TransactionRepository;

        var result = await repo.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ShouldReturnNull()
    {
        var repo = _fixture.TransactionRepository;

        var result = await repo.GetByIdAsync(9999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByAccountIdAsync_ValidAccountId_ShouldReturnAccountTransactions()
    {
        var repo = _fixture.TransactionRepository;

        var result = await repo.GetByAccountIdAsync(1);

        Assert.NotEmpty(result);
        Assert.All(result, t => Assert.Equal(1, t.AccountId));
    }

    [Fact]
    public async Task GetByAccountIdAsync_InvalidAccountId_ShouldReturnEmpty()
    {
        var repo = _fixture.TransactionRepository;

        var result = await repo.GetByAccountIdAsync(9999);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByAccountIdAsync_ShouldReturnSortedByCreatedAtDescending()
    {
        var repo = _fixture.TransactionRepository;

        var result = await repo.GetByAccountIdAsync(1);

        for (int i = 0; i < result.Count - 1; i++)
        {
            Assert.True(result[i].CreatedAt >= result[i + 1].CreatedAt);
        }
    }

    [Fact]
    public async Task CreateAsync_ShouldAssignId()
    {
        var repo = _fixture.TransactionRepository;

        var transaction = new Transaction
        {
            AccountId = 1,
            Type = "Deposit",
            Amount = 500m,
            CreatedAt = DateTime.UtcNow,
            Description = "Test"
        };

        var result = await repo.CreateAsync(transaction);

        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistTransaction()
    {
        var repo = _fixture.TransactionRepository;

        var transaction = new Transaction
        {
            AccountId = 1,
            Type = "Withdrawal",
            Amount = 250m,
            CreatedAt = DateTime.UtcNow,
            Description = "Test withdrawal"
        };

        var created = await repo.CreateAsync(transaction);
        var retrieved = await repo.GetByIdAsync(created.Id);

        Assert.NotNull(retrieved);
        Assert.Equal(created.Id, retrieved!.Id);
        Assert.Equal("Withdrawal", retrieved.Type);
        Assert.Equal(250m, retrieved.Amount);
    }

    [Fact]
    public async Task CreateAsync_MultipleTransactions_ShouldHaveIncrementingIds()
    {
        var repo = _fixture.TransactionRepository;

        var transaction1 = new Transaction { AccountId = 1, Type = "Deposit", Amount = 100m, CreatedAt = DateTime.UtcNow };
        var transaction2 = new Transaction { AccountId = 2, Type = "Withdrawal", Amount = 50m, CreatedAt = DateTime.UtcNow };

        var created1 = await repo.CreateAsync(transaction1);
        var created2 = await repo.CreateAsync(transaction2);

        Assert.True(created2.Id > created1.Id);
    }
}
