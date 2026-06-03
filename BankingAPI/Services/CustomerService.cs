using BankingAPI.Repositories;
using BankingAPI.Models;

namespace BankingAPI.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAccountRepository _accountRepository;

        public CustomerService(ICustomerRepository customerRepository, IAccountRepository accountRepository)
        {
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
        }

        private async Task<Customer?> LoadAccountsAsync(Customer? customer)
        {
            if (customer == null)
                return null;

            customer.Accounts = await _accountRepository.GetByCustomerIdAsync(customer.Id);
            return customer;
        }

        private async Task<List<Customer>> LoadAccountsAsync(IEnumerable<Customer> customers)
        {
            var result = new List<Customer>();

            foreach (var customer in customers)
            {
                var hydratedCustomer = await LoadAccountsAsync(customer);
                if (hydratedCustomer != null)
                {
                    result.Add(hydratedCustomer);
                }
            }

            return result;
        }

        // total balance service method 
        public async Task<decimal?> GetCustomerTotalBalanceAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
                return null;

            var accounts = await _accountRepository.GetByCustomerIdAsync(id);
            return accounts.Sum(a => a.Balance);
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await LoadAccountsAsync(await _customerRepository.GetAllAsync());
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            return await LoadAccountsAsync(await _customerRepository.GetByIdAsync(id));
        }

        public async Task<IEnumerable<Customer>> GetCustomerByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return await GetAllCustomersAsync();

            var customers = await _customerRepository.GetAllAsync();

            return await LoadAccountsAsync(customers
                .Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList());
        }

        // NEW
        public async Task<IEnumerable<Customer>> GetCustomerByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return await GetAllCustomersAsync();

            var customers = await _customerRepository.GetAllAsync();

            return await LoadAccountsAsync(customers
                .Where(c => c.Email.Contains(email, StringComparison.OrdinalIgnoreCase))
                .ToList());
        }

        //Step 5: Implement Filtered Logic (Premium Customers): For GetAllPremiumCustomers, implement
        // a filtering mechanism using loops or query expressions (e.g., LINQ, Streams, or array filters) to
        // calculate if the balance exceeds your defined numeric threshold (e.g., $10,000)
        public async Task<IEnumerable<Customer>> GetAllPremiumCustomersAsync(decimal? threshold)
        {
            decimal t = threshold ?? 10000m;
            var customers = await GetAllCustomersAsync();

            return customers
                .Where(c => c.Accounts.Sum(a => a.Balance) > t)
                .ToList();
        }

        public async Task<Customer?> CreateCustomerAsync(Customer customer)
        {
            // check duplicate email
            var customers = await _customerRepository.GetAllAsync();
            if (customers.Any(c =>
                c.Email.Equals(customer.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            customer.Accounts = customer.Accounts ?? new List<Account>();
            var created = await _customerRepository.CreateAsync(customer);

            foreach (var account in customer.Accounts)
            {
                await _accountRepository.CreateAsync(created.Id, account);
            }

            return await LoadAccountsAsync(created);
        }

        public async Task<Customer?> UpdateCustomerAsync(int id, Customer update)
        {
            var existing = await _customerRepository.GetByIdAsync(id);
            if (existing == null)
                return null;

            var updated = await _customerRepository.UpdateAsync(id, update);
            return await LoadAccountsAsync(updated);
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return false;

            await _accountRepository.DeleteByCustomerIdAsync(id);
            return await _customerRepository.DeleteAsync(id);
        }
    }
}