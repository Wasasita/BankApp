using Backend.Api.Models;

namespace Backend.Api.Repositories;

public interface ICustomerRepository
{
    List<Customer> GetAllCustomers();
}