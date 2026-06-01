namespace Backend.Api.Models;

public class SavingsAccount : Account
{
    public decimal InterestRate { get; set; }

    public SavingsAccount(string accountNumber, Customer accountHolder, decimal balance, decimal interestRate)
        : base(accountNumber, accountHolder, balance)
    {
        InterestRate = interestRate;
    }

    // Step 4 Requirement: Minimum $100 balance rule
    public override void Withdraw(decimal amount)
    {
        if (Balance - amount >= 100)
        {
            Balance -= amount;
        }
        else
        {
            System.Console.WriteLine("Transaction Denied: Savings accounts must maintain a minimum balance of $100.");
        }
    }
}