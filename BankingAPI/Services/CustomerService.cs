using BankingAPI.Data;
using BankingAPI.Models;

namespace BankingAPI.Services
{
    public class CustomerService
    {
        public IEnumerable<Customer> GetAllCustomers()
        {
            return DataStore.Customers;
        }

        public Customer? GetCustomerById(int id)
        {
            return DataStore.Customers.FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<Customer> GetCustomerByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return DataStore.Customers;

            return DataStore.Customers
                .Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        //Step 5: Implement Filtered Logic (Premium Customers): For GetAllPremiumCustomers, implement
        // a filtering mechanism using loops or query expressions (e.g., LINQ, Streams, or array filters) to
        // calculate if the balance exceeds your defined numeric threshold (e.g., $10,000)
        public IEnumerable<Customer> GetAllPremiumCustomers(decimal? threshold)
        {
            decimal t = threshold ?? 10000m;
            return DataStore.Customers
                .Where(c => c.Accounts.Sum(a => a.Balance) > t)
                .ToList();
        }

        public Customer CreateCustomer(Customer customer)
        {
            var newId = DataStore.Customers.Any() ? DataStore.Customers.Max(c => c.Id) + 1 : 1;
            customer.Id = newId;
            customer.Accounts = customer.Accounts ?? new List<Account>();
            DataStore.Customers.Add(customer);
            return customer;
        }

        public Customer? UpdateCustomer(int id, Customer update)
        {
            var existing = GetCustomerById(id);
            if (existing == null)
                return null;

            existing.Name = update.Name;
            existing.Email = update.Email;
            return existing;
        }

        public bool DeleteCustomer(int id)
        {
            var customer = GetCustomerById(id);
            if (customer == null)
                return false;

            var accountIds = customer.Accounts.Select(a => a.Id).ToList();
            DataStore.Accounts.RemoveAll(a => accountIds.Contains(a.Id));
            customer.Accounts.Clear();
            DataStore.Customers.Remove(customer);

            return true;
        }
    }
}