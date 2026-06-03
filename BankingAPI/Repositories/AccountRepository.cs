using BankingAPI.Data;
using BankingAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BankingAPI.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly IMongoCollection<Account> _accounts;

    public AccountRepository(IOptions<MongoDbSettings> settings)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);

        _accounts = database.GetCollection<Account>(settings.Value.AccountsCollectionName);
    }

    public async Task<List<Account>> GetAllAsync()
    {
        return await _accounts.Find(_ => true).ToListAsync();
    }

    public async Task<Account?> GetByIdAsync(int id)
    {
        return await _accounts.Find(account => account.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Account>> GetByCustomerIdAsync(int customerId)
    {
        return await _accounts.Find(account => account.CustomerId == customerId).ToListAsync();
    }

    public async Task<Account> CreateAsync(int customerId, Account account)
    {
        var lastAccount = await _accounts.Find(_ => true).SortByDescending(a => a.Id).FirstOrDefaultAsync();
        account.Id = lastAccount == null ? 1 : lastAccount.Id + 1;
        account.CustomerId = customerId;

        await _accounts.InsertOneAsync(account);
        return account;
    }

    public async Task<Account?> UpdateAsync(int id, Account account)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null)
            return null;

        account.Id = id;
        account.CustomerId = existing.CustomerId;

        await _accounts.ReplaceOneAsync(existingAccount => existingAccount.Id == id, account);
        return account;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _accounts.DeleteOneAsync(account => account.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> DeleteByCustomerIdAsync(int customerId)
    {
        var result = await _accounts.DeleteManyAsync(account => account.CustomerId == customerId);
        return result.DeletedCount > 0;
    }
}