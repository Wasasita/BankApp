using System;

namespace Backend.Api.Models;

public abstract class Account : ITransaction
{
    public string AccountNumber { get; set; }
    public Customer AccountHolder { get; set; }
    public decimal Balance { get; set; } 

    public Account(string accountNumber, Customer accountHolder, decimal balance)
    {
        AccountNumber = accountNumber;
        AccountHolder = accountHolder;
        Balance = balance;
    }

    public virtual void Deposit(decimal amount)
    {
        Balance += amount;
    }

    public abstract void Withdraw(decimal amount);

    public virtual void PrintReceipt()
    {
        Console.WriteLine("----------------------------------------");
        Console.WriteLine($"           TRANSACTION RECEIPT          ");
        Console.WriteLine("----------------------------------------");
        Console.WriteLine($"Account Number: {AccountNumber}");
        Console.WriteLine($"Current Balance: {Balance:C}");
        Console.WriteLine("----------------------------------------");
    }
}