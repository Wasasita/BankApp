namespace BankingAPI.Models;

public enum TransactionOperationResult
{
    Success,
    InvalidAmount,
    AccountNotFound,
    InsufficientFunds,
    SameAccountTransfer
}