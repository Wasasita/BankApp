using System.Collections.Generic;
using BankingAPI.Models;

namespace BankingAPI.Data
{
    public static class DataStore
    {
        // Step 3: Establish Mock Data Store: Create static global List collections in your application to hold seed data. 
        public static List<Account> Accounts { get; } = new List<Account>();
        public static List<Customer> Customers { get; } = new List<Customer>();

        static DataStore()
        {
            // Populate it with 3-4 default customers and accounts so you have active data to fetch right away during early testing.
            var acc1 = new Account { Id = 1, AccountNumber = "10000001", AccountType = "Checking", Balance = 1250.75m };
            var acc2 = new Account { Id = 2, AccountNumber = "10000002", AccountType = "Savings", Balance = 5400.00m };
            var acc3 = new Account { Id = 3, AccountNumber = "10000003", AccountType = "Checking", Balance = 230.50m };
            var acc4 = new Account { Id = 4, AccountNumber = "10000004", AccountType = "Savings", Balance = 10230.10m };

            Accounts.AddRange(new[] { acc1, acc2, acc3, acc4 });

            Customers.Add(new Customer
            {
                Id = 1,
                Name = "Alice Johnson",
                Email = "alice.johnson@example.com",
                Accounts = new List<Account> { acc1, acc2 }
            });

            Customers.Add(new Customer
            {
                Id = 2,
                Name = "Bob Smith",
                Email = "bob.smith@example.com",
                Accounts = new List<Account> { acc3 }
            });

            Customers.Add(new Customer
            {
                Id = 3,
                Name = "Carol Brown",
                Email = "carol.brown@example.com",
                Accounts = new List<Account> { acc4 }
            });
        }
    }
}