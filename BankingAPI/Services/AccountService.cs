using BankingAPI.Repositories;
using BankingAPI.Models;

namespace BankingAPI.Services
{
    public class AccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ICustomerRepository _customerRepository;

        public AccountService(IAccountRepository accountRepository, ICustomerRepository customerRepository)
        {
            _accountRepository = accountRepository;
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Account>> GetAllAccountsAsync()
        {
            return await _accountRepository.GetAllAsync();
        }

        public async Task<Account?> GetAccountByIdAsync(int id)
        {
            return await _accountRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Account>> GetAccountByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return await GetAllAccountsAsync();

            var customers = await _customerRepository.GetAllAsync();
            var matchingCustomerIds = customers
                .Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Id)
                .ToList();

            var accounts = new List<Account>();

            foreach (var customerId in matchingCustomerIds)
            {
                accounts.AddRange(await _accountRepository.GetByCustomerIdAsync(customerId));
            }

            return accounts;
        }

        public async Task<Account?> CreateAccountAsync(int customerId, Account account)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
                return null;

            return await _accountRepository.CreateAsync(customerId, account);
        }

        public async Task<Account?> UpdateAccountAsync(int id, Account update)
        {
            var existing = await _accountRepository.GetByIdAsync(id);
            if (existing == null)
                return null;

            return await _accountRepository.UpdateAsync(id, update);
        }

        public async Task<bool> DeleteAccountAsync(int id)
        {
            return await _accountRepository.DeleteAsync(id);
        }
    }
}