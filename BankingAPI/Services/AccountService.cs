using BankingAPI.Data;
using BankingAPI.Models;

namespace BankingAPI.Services
{
    public class AccountService
    {
        public IEnumerable<Account> GetAllAccounts()
        {
            return DataStore.Accounts;
        }

        public Account? GetAccountById(int id)
        {
            return DataStore.Accounts.FirstOrDefault(a => a.Id == id);
        }

        public IEnumerable<Account> GetAccountByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return DataStore.Accounts;

            return DataStore.Customers
                .Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .SelectMany(c => c.Accounts)
                .ToList();
        }

        public Account? CreateAccount(int customerId, Account account)
        {
            var customer = DataStore.Customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null)
                return null;

            var newId = DataStore.Accounts.Any() ? DataStore.Accounts.Max(a => a.Id) + 1 : 1;
            account.Id = newId;

            DataStore.Accounts.Add(account);
            customer.Accounts.Add(account);

            return account;
        }

        public Account? UpdateAccount(int id, Account update)
        {
            var existing = GetAccountById(id);
            if (existing == null)
                return null;

            existing.AccountNumber = update.AccountNumber;
            existing.AccountType = update.AccountType;
            existing.Balance = update.Balance;

            return existing;
        }

        public bool DeleteAccount(int id)
        {
            var account = GetAccountById(id);
            if (account == null)
                return false;

            DataStore.Accounts.Remove(account);
            foreach (var customer in DataStore.Customers)
                customer.Accounts.RemoveAll(a => a.Id == id);

            return true;
        }
    }
}