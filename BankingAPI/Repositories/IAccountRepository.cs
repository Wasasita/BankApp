using BankingAPI.Models;

namespace BankingAPI.Repositories
{
    public interface IAccountRepository
    {
        Task<List<Account>> GetAllAsync();

        Task<Account?> GetByIdAsync(int id);

        Task<List<Account>> GetByCustomerIdAsync(int customerId);

        Task<Account> CreateAsync(int customerId, Account account);

        Task<Account?> UpdateAsync(int id, Account account);

        Task<bool> DeleteAsync(int id);

        Task<bool> DeleteByCustomerIdAsync(int customerId);
    }
}