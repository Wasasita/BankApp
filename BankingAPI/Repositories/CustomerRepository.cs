using BankingAPI.Data;
using BankingAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BankingAPI.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IMongoCollection<Customer> _customers;

    public CustomerRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var db = client.GetDatabase(settings.Value.DatabaseName);
        _customers = db.GetCollection<Customer>(settings.Value.CustomersCollectionName);
    }

    public async Task<List<Customer>> GetAllAsync()
        => await _customers.Find(_ => true).ToListAsync();

    public async Task<Customer?> GetByIdAsync(int id)
        => await _customers.Find(c => c.Id == id).FirstOrDefaultAsync();

    public async Task<List<Customer>> SearchByNameAsync(string name)
    {
        return await _customers
            .Find(c => c.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<List<Customer>> SearchByEmailAsync(string email)
    {
        return await _customers
            .Find(c => c.Email.Contains(email))
            .ToListAsync();
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        var last = await _customers.Find(_ => true)
            .SortByDescending(c => c.Id)
            .FirstOrDefaultAsync();

        customer.Id = last == null ? 1 : last.Id + 1;
        customer.Accounts ??= new();

        await _customers.InsertOneAsync(customer);
        return customer;
    }

    public async Task<Customer?> UpdateAsync(int id, Customer customer)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null) return null;

        customer.Id = id;
        customer.Accounts = existing.Accounts;

        await _customers.ReplaceOneAsync(x => x.Id == id, customer);
        return customer;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _customers.DeleteOneAsync(x => x.Id == id);
        return result.DeletedCount > 0;
    }
}