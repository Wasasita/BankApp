using BankingAPI.Models;
using BankingAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly TransactionService _transactionService;

    public TransactionsController(TransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Transaction>>> GetAllTransactions()
    {
        var transactions = await _transactionService.GetAllAsync();
        return Ok(transactions);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Transaction>> GetTransactionById(int id)
    {
        var transaction = await _transactionService.GetByIdAsync(id);
        if (transaction == null)
        {
            return NotFound(new { message = "Transaction not found" });
        }

        return Ok(transaction);
    }

    [HttpGet("account/{accountId:int}")]
    public async Task<ActionResult<List<Transaction>>> GetByAccountId(int accountId)
    {
        var transactions = await _transactionService.GetByAccountIdAsync(accountId);
        return Ok(transactions);
    }

    [HttpPost("deposit")]
    public async Task<ActionResult> Deposit([FromBody] DepositRequest request)
    {
        var result = await _transactionService.DepositAsync(request.AccountId, request.Amount, request.Description);
        if (result == TransactionOperationResult.AccountNotFound)
        {
            return NotFound(new { message = "Account not found" });
        }

        if (result == TransactionOperationResult.InvalidAmount)
        {
            return BadRequest(new { message = "Amount must be greater than zero" });
        }

        return Ok(new { message = "Deposit completed" });
    }

    [HttpPost("withdraw")]
    public async Task<ActionResult> Withdraw([FromBody] WithdrawRequest request)
    {
        var result = await _transactionService.WithdrawAsync(request.AccountId, request.Amount, request.Description);
        if (result == TransactionOperationResult.AccountNotFound)
        {
            return NotFound(new { message = "Account not found" });
        }

        if (result == TransactionOperationResult.InvalidAmount)
        {
            return BadRequest(new { message = "Amount must be greater than zero" });
        }

        if (result == TransactionOperationResult.InsufficientFunds)
        {
            return BadRequest(new { message = "Insufficient funds" });
        }

        return Ok(new { message = "Withdrawal completed" });
    }

    [HttpPost("transfer")]
    public async Task<ActionResult> Transfer([FromBody] TransferRequest request)
    {
        var result = await _transactionService.TransferAsync(
            request.FromAccountId,
            request.ToAccountId,
            request.Amount,
            request.Description);

        if (result == TransactionOperationResult.AccountNotFound)
        {
            return NotFound(new { message = "One or both accounts were not found" });
        }

        if (result == TransactionOperationResult.InvalidAmount)
        {
            return BadRequest(new { message = "Amount must be greater than zero" });
        }

        if (result == TransactionOperationResult.SameAccountTransfer)
        {
            return BadRequest(new { message = "From and to accounts must be different" });
        }

        if (result == TransactionOperationResult.InsufficientFunds)
        {
            return BadRequest(new { message = "Insufficient funds" });
        }

        return Ok(new { message = "Transfer completed" });
    }
}
