using BankingAPI.Models;
using BankingAPI.Repositories;

namespace BankingAPI.Services;

public class AccountService
{
    private readonly IAccountRepository _repo;

    public AccountService(IAccountRepository repo)
    {
        _repo = repo;
    }

    public Task<List<Account>> GetAllAccounts()
        => _repo.GetAllAsync();

    public Task<Account?> GetAccountById(int id)
        => _repo.GetByIdAsync(id);

    public Task<List<Account>> GetAccountByName(string name)
        => _repo.GetAllAsync();

    public Task<Account> CreateAccount(int customerId, Account account)
        => _repo.CreateAsync(customerId, account);

    public Task<Account?> UpdateAccount(int id, Account account)
        => _repo.UpdateAsync(id, account);

    public Task<bool> DeleteAccount(int id)
        => _repo.DeleteAsync(id);
}