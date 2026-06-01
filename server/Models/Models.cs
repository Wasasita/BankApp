using System;
using System.Collections.Generic;

namespace Backend.Api.Models;

// Step 1: The Interface
// public interface ITransaction
// {
//     void PrintReceipt();
// }

// // Base User Class
// public class User
// {
//     public string Username { get; set; }
//     public string Password { get; set; }

//     public User(string username, string password)
//     {
//         Username = username;
//         Password = password;
//     }
// }

// // Step 2: Customer Class (Inherits User)
// public class Customer : User
// {
//     public int Id { get; set; }
//     public string FirstName { get; set; }
//     public string LastName { get; set; }
//     public List<Account> Accounts { get; set; } = new List<Account>();

//     // Constructor passing username/password up to the base User class
//     public Customer(int id, string firstName, string lastName, string username, string password) 
//         : base(username, password)
//     {
//         Id = id;
//         FirstName = firstName;
//         LastName = lastName;
//     }

//     public override string ToString()
//     {
//         return $"Customer ID: {Id}, Name: {FirstName} {LastName}, Total Accounts: {Accounts.Count}";
//     }
// }

// public class Admin : User
// {
//     public int Id { get; set; }

//     public Admin(int id, string username, string password) : base(username, password)
//     {
//         Id = id;
//     }
// }

// Step 3: Abstract Account Class (Implements ITransaction)
// public abstract class Account : ITransaction
// {
//     public string AccountNumber { get; set; }
//     public Customer AccountHolder { get; set; }
//     public decimal Balance { get; set; } // Using decimal as requested by Step 3

//     public Account(string accountNumber, Customer accountHolder, decimal balance)
//     {
//         AccountNumber = accountNumber;
//         AccountHolder = accountHolder;
//         Balance = balance;
//     }

//     public virtual void Deposit(decimal amount)
//     {
//         Balance += amount;
//     }

//     public abstract void Withdraw(decimal amount);

//     // Interface method requirement
//     public virtual void PrintReceipt()
//     {
//         Console.WriteLine($"Account: {AccountNumber} | Balance: {Balance:C}");
//     }
// }

// Step 4: Saving Account Class (Inherits Account)
// public class SavingsAccount : Account
// {
//     public decimal OverdraftLimit { get; set; }

//     public SavingsAccount(string accountNumber, Customer accountHolder, decimal balance, decimal overdraftLimit)
//         : base(accountNumber, accountHolder, balance)
//     {
//         OverdraftLimit = overdraftLimit;
//     }

//     public override void Withdraw(decimal amount)
//     {
//         if (Balance - amount >= -OverdraftLimit)
//         {
//             Balance -= amount;
//         }
//         else
//         {
//             Console.WriteLine("Transaction Denied: Overdraft limit exceeded.");
//         }
//     }
// }

// Step 5: Checking Account Class (Inherits Account)
// public class CheckingAccount : Account
// {
//     public decimal OverdraftLimit { get; set; }

//     public CheckingAccount(string accountNumber, Customer accountHolder, decimal balance, decimal overdraftLimit)
//         : base(accountNumber, accountHolder, balance)
//     {
//         OverdraftLimit = overdraftLimit;
//     }

//     public override void Withdraw(decimal amount)
//     {
//         if (Balance - amount >= -OverdraftLimit)
//         {
//             Balance -= amount;
//         }
//         else
//         {
//             Console.WriteLine("Transaction Denied: Overdraft limit exceeded.");
//         }
//     }
// }