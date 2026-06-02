using Backend.Api.Models;

namespace Backend.Api.Repositories;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly List<Customer> _customers;

    public CustomerRepository(IEnumerable<Customer> customers)
    {
        _customers = customers.ToList();
    }

    public List<Customer> GetAllCustomers()
    {
        return _customers.ToList();
    }
}