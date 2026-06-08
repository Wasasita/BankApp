using BankingAPI.Data;
using BankingAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BankingAPI.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly IMongoCollection<Transaction> _transactions;

    public TransactionRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var db = client.GetDatabase(settings.Value.DatabaseName);
        _transactions = db.GetCollection<Transaction>(settings.Value.TransactionsCollectionName);
    }

    public async Task<List<Transaction>> GetAllAsync()
        => await _transactions.Find(_ => true)
            .SortByDescending(t => t.CreatedAt)
            .ToListAsync();

    public async Task<List<Transaction>> GetByAccountIdAsync(int accountId)
        => await _transactions.Find(t => t.AccountId == accountId)
            .SortByDescending(t => t.CreatedAt)
            .ToListAsync();

    public async Task<Transaction?> GetByIdAsync(int id)
        => await _transactions.Find(t => t.Id == id).FirstOrDefaultAsync();

    public async Task<Transaction> CreateAsync(Transaction transaction)
    {
        var last = await _transactions.Find(_ => true)
            .SortByDescending(t => t.Id)
            .FirstOrDefaultAsync();

        transaction.Id = last == null ? 1 : last.Id + 1;

        await _transactions.InsertOneAsync(transaction);
        return transaction;
    }
}
