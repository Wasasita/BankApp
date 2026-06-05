using BankingAPI.Models;
using BankingAPI.Repositories;

//Service = business logic

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

    public async Task<List<Customer>> GetAllCustomers()
    {
        var customers = await _customerRepo.GetAllAsync();
        var accounts = await _accountRepo.GetAllAsync();

        PopulateAccounts(customers, accounts);
        return customers;
    }

    public async Task<Customer?> GetCustomerById(int id)
    {
        var customer = await _customerRepo.GetByIdAsync(id);
        if (customer == null) return null;

        customer.Accounts = await _accountRepo.GetByCustomerIdAsync(customer.Id);
        return customer;
    }

    public async Task<List<Customer>> GetCustomerByName(string name)
    {
        var customers = await _customerRepo.SearchByNameAsync(name);
        var accounts = await _accountRepo.GetAllAsync();

        PopulateAccounts(customers, accounts);
        return customers;
    }

    public async Task<List<Customer>> GetCustomerByEmail(string email)
    {
        var customers = await _customerRepo.SearchByEmailAsync(email);
        var accounts = await _accountRepo.GetAllAsync();

        PopulateAccounts(customers, accounts);
        return customers;
    }

    public async Task<decimal?> GetCustomerTotalBalance(int id)
    {
        var customer = await _customerRepo.GetByIdAsync(id);
        if (customer == null) return null;

        var accounts = await _accountRepo.GetByCustomerIdAsync(id);
        return accounts.Sum(a => a.Balance);
    }

    public async Task<List<Customer>> GetAllPremiumCustomers(decimal? threshold)
    {
        var customers = await _customerRepo.GetAllAsync();
        var accounts = await _accountRepo.GetAllAsync();

        var t = threshold ?? 10000m;

        PopulateAccounts(customers, accounts);

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

    private static void PopulateAccounts(List<Customer> customers, List<Account> accounts)
    {
        foreach (var customer in customers)
        {
            customer.Accounts = accounts
                .Where(a => a.CustomerId == customer.Id)
                .ToList();
        }
    }
}