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
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);

        _customers = database.GetCollection<Customer>(settings.Value.CustomersCollectionName);
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _customers.Find(_ => true).ToListAsync();
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        return await _customers.Find(customer => customer.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        var lastCustomer = await _customers.Find(_ => true).SortByDescending(c => c.Id).FirstOrDefaultAsync();
        customer.Id = lastCustomer == null ? 1 : lastCustomer.Id + 1;
        customer.Accounts = new List<Account>();
        await _customers.InsertOneAsync(customer);
        return customer;
    }

    public async Task<Customer?> UpdateAsync(int id, Customer customer)
    {
        var existing = await GetByIdAsync(id);
        if (existing == null)
            return null;

        customer.Id = id;
        customer.Accounts = existing.Accounts;

        await _customers.ReplaceOneAsync(existingCustomer => existingCustomer.Id == id, customer);
        return customer;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _customers.DeleteOneAsync(customer => customer.Id == id);
        return result.DeletedCount > 0;
    }
}