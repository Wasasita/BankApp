using BankingAPI.Models;
using BankingAPI.Repositories;

namespace BankingAPI.Services;

public class TransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICustomerRepository _customerRepository;

    public TransactionService(
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepository,
        ICustomerRepository customerRepository)
    {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
        _customerRepository = customerRepository;
    }

    public Task<List<Transaction>> GetAllAsync()
        => _transactionRepository.GetAllAsync();

    public Task<Transaction?> GetByIdAsync(int id)
        => _transactionRepository.GetByIdAsync(id);

    public Task<List<Transaction>> GetByAccountIdAsync(int accountId)
        => _transactionRepository.GetByAccountIdAsync(accountId);

    public async Task<TransactionOperationResult> DepositAsync(int accountId, decimal amount, string? description = null)
    {
        if (amount <= 0)
        {
            return TransactionOperationResult.InvalidAmount;
        }

        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null)
        {
            return TransactionOperationResult.AccountNotFound;
        }

        account.Balance += amount;
        await _accountRepository.UpdateAsync(account.Id, account);
        await UpdateCustomerEmbeddedAccountBalanceAsync(account);

        await _transactionRepository.CreateAsync(new Transaction
        {
            AccountId = accountId,
            Amount = amount,
            Type = "Deposit",
            CreatedAt = DateTime.UtcNow,
            Description = description
        });

        return TransactionOperationResult.Success;
    }

    public async Task<TransactionOperationResult> WithdrawAsync(int accountId, decimal amount, string? description = null)
    {
        if (amount <= 0)
        {
            return TransactionOperationResult.InvalidAmount;
        }

        var account = await _accountRepository.GetByIdAsync(accountId);
        if (account == null)
        {
            return TransactionOperationResult.AccountNotFound;
        }

        if (account.Balance < amount)
        {
            return TransactionOperationResult.InsufficientFunds;
        }

        account.Balance -= amount;
        await _accountRepository.UpdateAsync(account.Id, account);
        await UpdateCustomerEmbeddedAccountBalanceAsync(account);

        await _transactionRepository.CreateAsync(new Transaction
        {
            AccountId = accountId,
            Amount = amount,
            Type = "Withdrawal",
            CreatedAt = DateTime.UtcNow,
            Description = description
        });

        return TransactionOperationResult.Success;
    }

    public async Task<TransactionOperationResult> TransferAsync(int fromAccountId, int toAccountId, decimal amount, string? description = null)
    {
        if (amount <= 0)
        {
            return TransactionOperationResult.InvalidAmount;
        }

        if (fromAccountId == toAccountId)
        {
            return TransactionOperationResult.SameAccountTransfer;
        }

        var fromAccount = await _accountRepository.GetByIdAsync(fromAccountId);
        var toAccount = await _accountRepository.GetByIdAsync(toAccountId);

        if (fromAccount == null || toAccount == null)
        {
            return TransactionOperationResult.AccountNotFound;
        }

        if (fromAccount.Balance < amount)
        {
            return TransactionOperationResult.InsufficientFunds;
        }

        fromAccount.Balance -= amount;
        toAccount.Balance += amount;

        await _accountRepository.UpdateAsync(fromAccount.Id, fromAccount);
        await _accountRepository.UpdateAsync(toAccount.Id, toAccount);

        await UpdateCustomerEmbeddedAccountBalanceAsync(fromAccount);
        await UpdateCustomerEmbeddedAccountBalanceAsync(toAccount);

        await _transactionRepository.CreateAsync(new Transaction
        {
            AccountId = fromAccountId,
            Amount = amount,
            Type = "Transfer",
            CreatedAt = DateTime.UtcNow,
            Description = string.IsNullOrWhiteSpace(description)
                ? $"Transfer to account {toAccountId}"
                : description
        });

        await _transactionRepository.CreateAsync(new Transaction
        {
            AccountId = toAccountId,
            Amount = amount,
            Type = "Transfer",
            CreatedAt = DateTime.UtcNow,
            Description = string.IsNullOrWhiteSpace(description)
                ? $"Transfer from account {fromAccountId}"
                : description
        });

        return TransactionOperationResult.Success;
    }

    private async Task UpdateCustomerEmbeddedAccountBalanceAsync(Account account)
    {
        var owner = await _customerRepository.GetByIdAsync(account.CustomerId);
        if (owner == null)
        {
            return;
        }

        var ownerAccount = owner.Accounts.FirstOrDefault(a => a.Id == account.Id);
        if (ownerAccount == null)
        {
            return;
        }

        ownerAccount.Balance = account.Balance;
        await _customerRepository.UpdateAsync(owner.Id, owner);
    }
}
