using BankingAPI.Models;
using BankingAPI.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankingAPI.Services;

public class AccountService
{
    private readonly IAccountRepository _accountRepo;
    private readonly ICustomerRepository _customerRepo;

    public AccountService(IAccountRepository accountRepo, ICustomerRepository customerRepo)
    {
        _accountRepo = accountRepo;
        _customerRepo = customerRepo;
    }

    public async Task<List<AccountResponse>> GetAllAccountsAsync()
    {
        var accounts = await _accountRepo.GetAllAsync();
        var customers = await _customerRepo.GetAllAsync();
        var customerNames = customers.ToDictionary(c => c.Id, c => c.Name);

        return accounts
            .Select(account => BuildAccountResponse(account, customerNames.GetValueOrDefault(account.CustomerId, string.Empty)))
            .ToList();
    }

    public async Task<AccountResponse?> GetAccountByIdAsync(int id)
    {
        var account = await _accountRepo.GetByIdAsync(id);
        if (account == null)
            return null;

        var owner = await _customerRepo.GetByIdAsync(account.CustomerId);
        return BuildAccountResponse(account, owner?.Name ?? string.Empty);
    }

    public async Task<List<AccountResponse>> SearchAccountsByCustomerNameAsync(string? name)
    {
        var customers = await _customerRepo.GetAllAsync();
        var matchingCustomers = customers
            .Where(c => string.IsNullOrWhiteSpace(name) || c.Name.Contains(name, System.StringComparison.OrdinalIgnoreCase))
            .ToList();

        return matchingCustomers
            .SelectMany(c => c.Accounts)
            .Select(account => BuildAccountResponse(account, matchingCustomers.FirstOrDefault(c => c.Id == account.CustomerId)?.Name ?? string.Empty))
            .OrderBy(account => account.Id)
            .ToList();
    }

    public async Task<Account?> CreateAccountAsync(int customerId, Account account)
    {
        var customer = await _customerRepo.GetByIdAsync(customerId);
        if (customer == null)
            return null;

        var created = await _accountRepo.CreateAsync(customerId, account);
        if (created != null)
        {
            customer.Accounts.Add(created);
            await _customerRepo.UpdateAsync(customerId, customer);
        }

        return created;
    }

    public async Task<Account?> UpdateAccountAsync(int id, Account account)
    {
        var existing = await _accountRepo.GetByIdAsync(id);
        if (existing == null)
            return null;

        account.Id = id;
        account.CustomerId = existing.CustomerId;
        var updated = await _accountRepo.UpdateAsync(id, account);
        if (updated != null)
        {
            var owner = await _customerRepo.GetByIdAsync(updated.CustomerId);
            if (owner != null)
            {
                var ownerAccount = owner.Accounts.FirstOrDefault(a => a.Id == id);
                if (ownerAccount != null)
                {
                    ownerAccount.AccountNumber = updated.AccountNumber;
                    ownerAccount.AccountType = updated.AccountType;
                    ownerAccount.Balance = updated.Balance;
                    await _customerRepo.UpdateAsync(owner.Id, owner);
                }
            }
        }

        return updated;
    }

    public async Task<bool> DeleteAccountAsync(int id)
    {
        var existing = await _accountRepo.GetByIdAsync(id);
        if (existing == null)
            return false;

        var deleted = await _accountRepo.DeleteAsync(id);
        if (!deleted)
            return false;

        var owner = await _customerRepo.GetByIdAsync(existing.CustomerId);
        if (owner != null)
        {
            owner.Accounts.RemoveAll(a => a.Id == id);
            await _customerRepo.UpdateAsync(owner.Id, owner);
        }

        return true;
    }

    private static AccountResponse BuildAccountResponse(Account account, string customerName)
        => new()
        {
            Id = account.Id,
            CustomerId = account.CustomerId,
            AccountNumber = account.AccountNumber,
            AccountType = account.AccountType,
            Balance = account.Balance,
            CustomerName = customerName
        };
}
