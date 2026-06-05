using BankingAPI.Data;
using BankingAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

// Repository = database access

namespace BankingAPI.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly IMongoCollection<Account> _accounts;

    public AccountRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var db = client.GetDatabase(settings.Value.DatabaseName);
        _accounts = db.GetCollection<Account>(settings.Value.AccountsCollectionName);
    }

    public async Task<List<Account>> GetAllAsync()
        => await _accounts.Find(_ => true).ToListAsync();

    public async Task<Account?> GetByIdAsync(int id)
        => await _accounts.Find(a => a.Id == id).FirstOrDefaultAsync();

    public async Task<List<Account>> GetByCustomerIdAsync(int customerId)
        => await _accounts.Find(a => a.CustomerId == customerId).ToListAsync();

    public async Task<Account> CreateAsync(int customerId, Account account)
    {
        var last = await _accounts.Find(_ => true)
            .SortByDescending(a => a.Id)
            .FirstOrDefaultAsync();

        account.Id = last == null ? 1 : last.Id + 1;
        account.CustomerId = customerId;

        await _accounts.InsertOneAsync(account);
        return account;
    }

    public async Task<Account?> UpdateAsync(int id, Account account)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null) return null;

        account.Id = id;
        account.CustomerId = existing.CustomerId;

        await _accounts.ReplaceOneAsync(x => x.Id == id, account);
        return account;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _accounts.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> DeleteByCustomerIdAsync(int customerId)
    {
        var result = await _accounts.DeleteManyAsync(x => x.CustomerId == customerId);
        return result.DeletedCount > 0;
    }
}