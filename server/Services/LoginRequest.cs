using System;
using System.Collections.Generic;
using Backend.Api.Models;

namespace Backend.Api.Services;

public sealed class LoginService
{
    // The static list acts like your in-memory database
    private static List<Customer> customers = new List<Customer>();

    // This is the static constructor that seeds your data
    static LoginService()
    {
        // We will define the Customer class below!
        Customer c1 = new Customer(1, "Admin", "User", "admin", "secret123");
        customers.Add(c1);
    }

    // Your backend Web API login endpoint will call this
    public bool VerifyLoginFromWeb(string username, string password)
    {
        foreach (var customer in customers)
        {
            // Change customer.Name to customer.Username to match the new User base structure!
            if (customer.Username == username && customer.Password == password)
            {
                return true;
            }
        }
        return false;
    }

    // If you ever need to run his exact Console-style login for training:
    public static void ConsoleLogin()
    {
        Console.WriteLine("Please enter Username and password, space separated:");
        
        // FIXES CS8600: The ?? "" ensures it is never a null literal
        string input = Console.ReadLine() ?? ""; 
        
        string[] usernamePassword = input.Split(' '); 
        
        if (usernamePassword.Length >= 2)
        {
            string username = usernamePassword[0];
            string password = usernamePassword[1];
            Console.WriteLine($"Attempting login for: {username}");
        }
    }
}