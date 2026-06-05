using BankingAPI.Models;

namespace BankingAPI.Repositories
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int id);
        Task<List<Customer>> SearchByNameAsync(string name);
        Task<List<Customer>> SearchByEmailAsync(string email);
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer?> UpdateAsync(int id, Customer customer);
        Task<bool> DeleteAsync(int id);
    }
}