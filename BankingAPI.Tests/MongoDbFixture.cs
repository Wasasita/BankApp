using BankingAPI.Data;
using BankingAPI.Models;
using BankingAPI.Repositories;
using BankingAPI.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

public class MongoDbFixture : IDisposable
{
    public IMongoClient Client { get; }
    public IMongoDatabase Database { get; }
    public ICustomerRepository CustomerRepository { get; }
    public IAccountRepository AccountRepository { get; }
    public CustomerService CustomerService { get; }
    public AccountService AccountService { get; }
    private readonly MongoDbSettings _settings;

    public MongoDbFixture()
    {
        _settings = new MongoDbSettings
        {
            ConnectionString = Environment.GetEnvironmentVariable("MongoDbSettings__ConnectionString") ?? "mongodb://localhost:27017",
            DatabaseName = (Environment.GetEnvironmentVariable("MongoDbSettings__DatabaseName") ?? "BankApp") + "_tests"
        };

        Client = new MongoClient(_settings.ConnectionString);
        Database = Client.GetDatabase(_settings.DatabaseName);

        var options = Options.Create(_settings);
        CustomerRepository = new CustomerRepository(options);
        AccountRepository = new AccountRepository(options);
        CustomerService = new CustomerService(CustomerRepository, AccountRepository);
        AccountService = new AccountService(AccountRepository, CustomerRepository);

        ResetDatabase();
        SeedDatabase();
    }

    private void ResetDatabase()
    {
        Database.DropCollection(_settings.CustomersCollectionName);
        Database.DropCollection(_settings.AccountsCollectionName);
    }

    private void SeedDatabase()
    {
        var accountsCollection = Database.GetCollection<Account>(_settings.AccountsCollectionName);
        var customersCollection = Database.GetCollection<Customer>(_settings.CustomersCollectionName);

        var account1 = new Account { Id = 1, CustomerId = 1, AccountNumber = "10000001", AccountType = "Checking", Balance = 1250.75m };
        var account2 = new Account { Id = 2, CustomerId = 1, AccountNumber = "10000002", AccountType = "Savings", Balance = 5400.00m };
        var account3 = new Account { Id = 3, CustomerId = 2, AccountNumber = "10000003", AccountType = "Checking", Balance = 230.50m };
        var account4 = new Account { Id = 4, CustomerId = 3, AccountNumber = "10000004", AccountType = "Savings", Balance = 10230.10m };

        accountsCollection.InsertMany(new[] { account1, account2, account3, account4 });

        var customer1 = new Customer
        {
            Id = 1,
            Name = "Alice Johnson",
            Email = "alice.johnson@example.com",
            Accounts = new List<Account> { account1, account2 }
        };

        var customer2 = new Customer
        {
            Id = 2,
            Name = "Bob Smith",
            Email = "bob.smith@example.com",
            Accounts = new List<Account> { account3 }
        };

        var customer3 = new Customer
        {
            Id = 3,
            Name = "Carol Brown",
            Email = "carol.brown@example.com",
            Accounts = new List<Account> { account4 }
        };

        customersCollection.InsertMany(new[] { customer1, customer2, customer3 });
    }

    public void Dispose()
    {
        ResetDatabase();
    }
}

[CollectionDefinition("MongoDb collection")]
public class MongoDbCollection : ICollectionFixture<MongoDbFixture>
{
}
