using BankingAPI.Models;
using BankingAPI.Repositories;

namespace BankingAPI.Services;

public class CustomerService
{
    private readonly ICustomerRepository _customerRepo;
    private readonly IAccountRepository _accountRepo;

    public CustomerService(ICustomerRepository customerRepo, IAccountRepository accountRepo)
    {
        _customerRepo = customerRepo;
        _accountRepo = accountRepo;
    }

    public Task<List<Customer>> GetAllCustomers()
        => _customerRepo.GetAllAsync();

    public Task<Customer?> GetCustomerById(int id)
        => _customerRepo.GetByIdAsync(id);

    public Task<List<Customer>> GetCustomerByName(string name)
        => _customerRepo.GetAllAsync();

    public Task<List<Customer>> GetCustomerByEmail(string email)
        => _customerRepo.GetAllAsync();

    public async Task<decimal?> GetCustomerTotalBalance(int id)
    {
        var customer = await _customerRepo.GetByIdAsync(id);
        if (customer == null) return null;

        return customer.Accounts.Sum(a => a.Balance);
    }

    public async Task<List<Customer>> GetAllPremiumCustomers(decimal? threshold)
    {
        var customers = await _customerRepo.GetAllAsync();
        var t = threshold ?? 10000m;

        return customers
            .Where(c => c.Accounts.Sum(a => a.Balance) > t)
            .ToList();
    }

    public Task<Customer> CreateCustomer(Customer customer)
        => _customerRepo.CreateAsync(customer);

    public Task<Customer?> UpdateCustomer(int id, Customer update)
        => _customerRepo.UpdateAsync(id, update);

    public Task<bool> DeleteCustomer(int id)
        => _customerRepo.DeleteAsync(id);
}