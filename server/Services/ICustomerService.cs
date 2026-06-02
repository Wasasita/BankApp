using Backend.Api.Models;

namespace Backend.Api.Services;

public interface ICustomerService
{
    List<Customer> GetAllCustomers();
}