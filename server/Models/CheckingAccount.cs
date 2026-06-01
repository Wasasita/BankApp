namespace Backend.Api.Models;

public class CheckingAccount : Account
{
    public decimal OverdraftLimit { get; set; }

    public CheckingAccount(string accountNumber, Customer accountHolder, decimal balance, decimal overdraftLimit)
        : base(accountNumber, accountHolder, balance)
    {
        OverdraftLimit = overdraftLimit;
    }

    // Step 5 Requirement: Overdraft protection rule
    public override void Withdraw(decimal amount)
    {
        if (Balance - amount >= -OverdraftLimit)
        {
            Balance -= amount;
        }
        else
        {
            System.Console.WriteLine("Transaction Denied: Overdraft limit exceeded.");
        }
    }
}