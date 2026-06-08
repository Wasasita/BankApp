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
using System.Security.Cryptography;
using System.Text;

public class MongoDbFixture : IDisposable
{
    public IMongoClient Client { get; }
    public IMongoDatabase Database { get; }
    public ICustomerRepository CustomerRepository { get; }
    public IAccountRepository AccountRepository { get; }
    public ITransactionRepository TransactionRepository { get; }
    public IUserRepository UserRepository { get; }
    public CustomerService CustomerService { get; }
    public AccountService AccountService { get; }
    public TransactionService TransactionService { get; }
    public AuthService AuthService { get; }
    private readonly MongoDbSettings _settings;
    private readonly JwtSettings _jwtSettings;

    public MongoDbFixture()
    {
        _settings = new MongoDbSettings
        {
            ConnectionString = Environment.GetEnvironmentVariable("MongoDbSettings__ConnectionString") ?? "mongodb://localhost:27017",
            DatabaseName = (Environment.GetEnvironmentVariable("MongoDbSettings__DatabaseName") ?? "BankApp") + "_tests"
        };

        _jwtSettings = new JwtSettings
        {
            Issuer = "BankingAPI",
            Audience = "BankingAPI.Frontend",
            SecretKey = "test_secret_key_for_unit_tests_1234567890",
            ExpirationMinutes = 60
        };

        Client = new MongoClient(_settings.ConnectionString);
        Database = Client.GetDatabase(_settings.DatabaseName);

        var options = Options.Create(_settings);
        var jwtOptions = Options.Create(_jwtSettings);

        CustomerRepository = new CustomerRepository(options);
        AccountRepository = new AccountRepository(options);
        TransactionRepository = new TransactionRepository(options);
        UserRepository = new UserRepository(options);

        CustomerService = new CustomerService(CustomerRepository, AccountRepository);
        AccountService = new AccountService(AccountRepository, CustomerRepository);
        TransactionService = new TransactionService(TransactionRepository, AccountRepository, CustomerRepository);
        AuthService = new AuthService(UserRepository, jwtOptions);

        ResetDatabase();
        SeedDatabase();
    }

    private void ResetDatabase()
    {
        Database.DropCollection(_settings.CustomersCollectionName);
        Database.DropCollection(_settings.AccountsCollectionName);
        Database.DropCollection(_settings.TransactionsCollectionName);
        Database.DropCollection(_settings.UsersCollectionName);
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

        // Seed transactions
        var transactionsCollection = Database.GetCollection<Transaction>(_settings.TransactionsCollectionName);
        var transaction1 = new Transaction { Id = 1, AccountId = 1, Type = "Deposit", Amount = 1000, CreatedAt = DateTime.UtcNow, Description = "Initial deposit" };
        var transaction2 = new Transaction { Id = 2, AccountId = 1, Type = "Withdrawal", Amount = 100, CreatedAt = DateTime.UtcNow.AddMinutes(-10), Description = null };
        var transaction3 = new Transaction { Id = 3, AccountId = 2, Type = "Transfer", Amount = 500, CreatedAt = DateTime.UtcNow.AddMinutes(-20), Description = "Transfer from account 1" };
        transactionsCollection.InsertMany(new[] { transaction1, transaction2, transaction3 });

        // Seed users (with SHA-256 hashed passwords)
        var usersCollection = Database.GetCollection<User>(_settings.UsersCollectionName);
        var adminPasswordHash = ComputeSha256Hash("admin123");
        var userPasswordHash = ComputeSha256Hash("user123");
        var admin = new User { Id = 1, Username = "admin", PasswordHash = adminPasswordHash, Role = "Admin" };
        var user = new User { Id = 2, Username = "testuser", PasswordHash = userPasswordHash, Role = "User" };
        usersCollection.InsertMany(new[] { admin, user });
    }

    private static string ComputeSha256Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
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
