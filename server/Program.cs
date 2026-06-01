using Backend.Api.Services;
using Backend.Api.Models;
using System.Collections.Generic;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<AddNumbersService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors("Frontend");

// Web API Endpoints
app.MapGet("/api/add", (int a, int b, AddNumbersService service) =>
{
    var result = service.Add(a, b);
    return Results.Ok(new AddNumbersResponse(a, b, result));
});

app.MapGet("/api/welcome", () =>
{
    return Results.Ok(new { Message = "Hello, World!" });
});

// ========================================================
// TERMINAL CONSOLE LOOP & DATA SEEDING (FOR TRAINING)
// ========================================================
int counter = 1;
List<User> users = new List<User>();
List<Customer> customers = new List<Customer>();

// Seed Customers
Customer c1 = new Customer(counter++, "John", "Doe", "john_doe", "secure456");
Customer c2 = new Customer(counter++, "Jane", "Smith", "jane_smith", "password789");

// Seed Accounts directly inside those customers to fix the "0 Total Accounts" bug!
CheckingAccount acc1 = new CheckingAccount("CH-1001", c1, 250.00m, 100.00m);
SavingsAccount acc2 = new SavingsAccount("SV-2002", c1, 1500.00m, 0.02m);
c1.Accounts.Add(acc1);
c1.Accounts.Add(acc2);

CheckingAccount acc3 = new CheckingAccount("CH-3003", c2, 50.00m, 50.00m);
c2.Accounts.Add(acc3);

// Save to databases
customers.Add(c1);
customers.Add(c2);
users.Add(c1);
users.Add(c2);

// PRINT HELLO WORLD BEFORE ACCESSING MAIN APPLICATION
Console.Clear();
Console.WriteLine("======================================");
Console.WriteLine("  Hello World - CitiBank Console App  ");
Console.WriteLine("======================================\n");
Console.WriteLine("Press any key to load the main menu...");
Console.ReadKey();

bool running = true;
while (running)
{
    Console.Clear();
    Console.WriteLine("=== MASTER BANKING MENU ===");
    Console.WriteLine("1) Login to Account Dashboard");
    Console.WriteLine("2) View All Active Customers");
    Console.WriteLine("7) Stop Menu & Start Web Server");
    Console.Write("Select an option: ");
    
    string choice = Console.ReadLine() ?? "";
    switch (choice)
    {
        case "1":
            string loginResult = HandleConsoleLogin(customers);
            if (loginResult != "validation_failed")
            {
                CustomerDashboard(loginResult, customers);
            }
            else
            {
                Console.WriteLine("\nLogin Failed! Returning to Main Menu.");
                Console.ReadKey();
            }
            break;
        case "2":
            Console.WriteLine("\n=== Current Registered Customers ===");
            foreach (var customer in customers)
            {
                Console.WriteLine(customer.ToString());
            }
            Console.ReadKey();
            break;
        case "7":
            running = false;
            Console.WriteLine("\nStarting Web Host Server backend...");
            break;
    }
}

app.Run();

// ========================================================
// HELPER METHODS FOR ROUTING TRAFFIC
// ========================================================
string HandleConsoleLogin(List<Customer> customerDatabase)
{
    Console.Clear();
    Console.WriteLine("=== LOGIN SYSTEM ===");
    Console.WriteLine("Please enter Username and Password (separated by a space):");
    
    string input = Console.ReadLine() ?? "";
    string[] credentials = input.Split(' ');

    if (credentials.Length < 2) return "validation_failed";

    string typedUsername = credentials[0];
    string typedPassword = credentials[1];

    foreach (var customer in customerDatabase)
    {
        if (customer.Username == typedUsername && customer.Password == typedPassword)
        {
            return customer.Username;
        }
    }
    return "validation_failed";
}

void CustomerDashboard(string loggedInUser, List<Customer> customerDatabase)
{
    Customer? currentCustomer = customerDatabase.Find(c => c.Username == loggedInUser);

    bool inDashboard = true;
    while (inDashboard)
    {
        Console.Clear();
        Console.WriteLine($"=== CUSTOMER DASHBOARD ===");
        Console.WriteLine($"Welcome back, {currentCustomer?.FirstName} {currentCustomer?.LastName}!\n");
        Console.WriteLine("1) View Account Balance");
        Console.WriteLine("2) Print Transaction Receipt");
        Console.WriteLine("3) Logout");
        Console.Write("Select an action: ");

        string choice = Console.ReadLine() ?? "";
        switch (choice)
        {
            case "1":
                Console.WriteLine("\n=== Your Account Balances ===");
                if (currentCustomer?.Accounts.Count == 0)
                {
                    Console.WriteLine("No balances found. You do not own any open banking containers yet.");
                }
                else
                {
                    foreach (var acc in currentCustomer!.Accounts)
                    {
                        // Determines sub-type naming strings dynamically
                        string type = acc is CheckingAccount ? "Checking" : "Savings";
                        Console.WriteLine($"-> [{type}] Acc Num: {acc.AccountNumber} | Balance: {acc.Balance:C}");
                    }
                }
                Console.ReadKey();
                break;
            case "2":
                Console.WriteLine("\n=== Print Polymorphic Receipts ===");
                if (currentCustomer?.Accounts.Count == 0)
                {
                    Console.WriteLine("No registered accounts available to read receipts from.");
                }
                else
                {
                    // Call PrintReceipt contracts using Abstraction rules cleanly
                    foreach (var acc in currentCustomer!.Accounts)
                    {
                        acc.PrintReceipt(); 
                    }
                }
                Console.ReadKey();
                break;
            case "3":
                Console.WriteLine("\nLogging out securely...");
                inDashboard = false;
                Console.ReadKey();
                break;
        }
    }
}

public sealed record AddNumbersResponse(int A, int B, int Result);