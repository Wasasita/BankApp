using Backend.Api.Services;
using Backend.Api.Models;

public class Program
{
    public static void Main(string[] args)
    {
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

        // TERMINAL CONSOLE LOOP & DATA SEEDING (FOR TRAINING PURPOSES ONLY - NOT FOR PRODUCTION USE!)
        int customerCounter = 1;
        int accountCounter = 1001;
        List<User> users = new List<User>();
        List<Customer> customers = new List<Customer>();

        // Seed Admins and Customers into the global users collection
        Admin admin = new Admin(99, "admin", "secret123");
        users.Add(admin);

        Customer c1 = new Customer(customerCounter++, "John", "Doe", "john_doe", "secure456");
        Customer c2 = new Customer(customerCounter++, "Jane", "Smith", "jane_smith", "password789");

        // Seed Accounts directly inside those customers to fix the "0 Total Accounts" bug!
        CheckingAccount acc1 = new CheckingAccount($"CH-{accountCounter++}", c1, 250.00m, 100.00m);
        SavingsAccount acc2 = new SavingsAccount($"SV-{accountCounter++}", c1, 1500.00m, 0.02m);
        c1.Accounts.Add(acc1);
        c1.Accounts.Add(acc2);

        CheckingAccount acc3 = new CheckingAccount($"CH-{accountCounter++}", c2, 50.00m, 50.00m);
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
            Console.WriteLine("7) Stop Menu & Start Web Server");
            Console.Write("Select an option: ");

            string choice = Console.ReadLine() ?? "";
            switch (choice)
            {
                case "1":
                    User? loggedInUser = HandleConsoleLogin(users);
                    if (loggedInUser != null)
                    {
                        if (loggedInUser is Admin standardAdmin)
                        {
                            AdminDashboard(standardAdmin, customers);
                        }
                        else if (loggedInUser is Customer standardCustomer)
                        {
                            CustomerDashboard(standardCustomer, customers, ref accountCounter);
                        }
                    }
                    else
                    {
                        Console.WriteLine("\nLogin Failed! Invalid credentials.");
                        Console.ReadKey();
                    }
                    break;

                case "7":
                    running = false;
                    Console.WriteLine("\nStarting Web Host Server backend...");
                    break;
            }
        }

        app.Run();
    }

    // ========================================================
    // HELPER METHODS FOR ROUTING TRAFFIC
    // ========================================================
    // ========================================================
    // SECURITY LOGIN SERVICE
    // ========================================================
    private static User? HandleConsoleLogin(List<User> database)
    {
        Console.Clear();
        Console.WriteLine("=== SYSTEM LOGIN ===");
        Console.Write("Enter Username and Password separated by a space: ");

        string input = Console.ReadLine() ?? "";
        string[] credentials = input.Split(' ');

        if (credentials.Length < 2) return null;

        string username = credentials[0];
        string password = credentials[1];

        return database.Find(u => u.Username == username && u.Password == password);
    }

    // ========================================================
    // ADMIN SYSTEM DASHBOARD
    // ========================================================
    private static void AdminDashboard(Admin currentAdmin, List<Customer> customerDatabase)
    {
        bool inAdmin = true;
        while (inAdmin)
        {
            Console.Clear();
            Console.WriteLine("=== CENTRAL ADMIN CONTROL PANEL ===");
            Console.WriteLine($"Logged in as System Admin: {currentAdmin.Username}\n");
            Console.WriteLine("1) View All Global Accounts");
            Console.WriteLine("2) Logout");
            Console.Write("Select an action: ");

            string choice = Console.ReadLine() ?? "";
            if (choice == "1")
            {
                Console.WriteLine("\n=== GLOBAL ACCOUNT AUDIT DATABASE ===");
                foreach (var customer in customerDatabase)
                {
                    Console.WriteLine($"Customer: {customer.FirstName} {customer.LastName} (ID: {customer.Id})");
                    if (customer.Accounts.Count == 0)
                    {
                        Console.WriteLine("  [No open financial accounts found]");
                    }

                    foreach (var acc in customer.Accounts)
                    {
                        string type = acc is CheckingAccount ? "Checking" : "Savings";
                        Console.WriteLine($"  -> [{type}] Base Num: {acc.AccountNumber} | Balance: {acc.Balance:C}");
                    }
                }
                Console.ReadKey();
            }
            else if (choice == "2")
            {
                inAdmin = false;
            }
        }
    }

    // ========================================================
    // CUSTOMER TRANSPARENT DASHBOARD
    // ========================================================
    private static void CustomerDashboard(Customer currentCustomer, List<Customer> customerDatabase, ref int globalAccountSequence)
    {
        bool inDashboard = true;
        while (inDashboard)
        {
            Console.Clear();
            Console.WriteLine($"=== CITIBANK CUSTOMER DASHBOARD ===");
            Console.WriteLine($"Welcome back, {currentCustomer.FirstName} {currentCustomer.LastName}!");
            Console.WriteLine($"Total Accounts Registered: {currentCustomer.Accounts.Count}\n");
            Console.WriteLine("1) Create Account");
            Console.WriteLine("2) View All Accounts");
            Console.WriteLine("3) Deposit");
            Console.WriteLine("4) Withdraw");
            Console.WriteLine("5) Transfer");
            Console.WriteLine("6) Close Account");
            Console.WriteLine("7) Logout");
            Console.Write("Select an action: ");

            string choice = Console.ReadLine() ?? "";
            switch (choice)
            {
                case "1": // 1) Create Account
                    Console.Clear();
                    Console.WriteLine("========================================");
                    Console.WriteLine("        CREATE NEW ACCOUNT        ");
                    Console.WriteLine("========================================");
                    Console.WriteLine("Select Type: 1) Checking Account  2) Savings Account");
                    Console.Write("Choice: ");
                    string typeChoice = Console.ReadLine() ?? "";

                    Console.Write("Enter Initial Deposit Amount: ");
                    decimal.TryParse(Console.ReadLine(), out decimal initialDeposit);

                    if (typeChoice == "1")
                    {
                        string accNum = $"CH-{globalAccountSequence++}";
                        CheckingAccount newAcc = new CheckingAccount(accNum, currentCustomer, initialDeposit, 100.00m);
                        currentCustomer.Accounts.Add(newAcc);
                        Console.WriteLine($"\n[SUCCESS] Checking Account {accNum} successfully established.");
                    }
                    else if (typeChoice == "2")
                    {
                        string accNum = $"SV-{globalAccountSequence++}";
                        SavingsAccount newAcc = new SavingsAccount(accNum, currentCustomer, initialDeposit, 0.02m);
                        currentCustomer.Accounts.Add(newAcc);
                        Console.WriteLine($"\n[SUCCESS] Savings Account {accNum} successfully established.");
                    }
                    Console.ReadKey();
                    break;

                case "2": // 2) View All Accounts (Pure Read Layout - No Receipts)
                    Console.Clear();
                    Console.WriteLine("=========================================================");
                    Console.WriteLine("               VIEW ALL ACCOUNTS             ");
                    Console.WriteLine("=========================================================");
                    if (currentCustomer.Accounts.Count == 0)
                    {
                        Console.WriteLine("No active accounts registered under this profile.");
                    }
                    else
                    {
                        Console.WriteLine($"{"Account Type",-15} | {"Account Number",-15} | {"Current Balance",-15}");
                        Console.WriteLine($"---------------------------------------------------------");
                        foreach (var acc in currentCustomer.Accounts)
                        {
                            string type = acc is CheckingAccount ? "Checking" : "Savings";
                            Console.WriteLine($"{type,-15} | {acc.AccountNumber,-15} | {acc.Balance,15:C}");
                        }
                        Console.WriteLine($"---------------------------------------------------------");
                    }
                    Console.ReadKey();
                    break;

                case "3": // 3) Deposit (Triggers Step 13 Receipt Confirmation)
                    Console.Clear();
                    Console.WriteLine("========================================");
                    Console.WriteLine("            FUNDS DEPOSIT SYSTEM        ");
                    Console.WriteLine("========================================");
                    Console.Write("Enter Target Account Number: ");
                    string targetDepositNum = Console.ReadLine() ?? "";

                    Account? depositAccount = currentCustomer.Accounts.Find(a => a.AccountNumber.Equals(targetDepositNum, StringComparison.OrdinalIgnoreCase));
                    if (depositAccount != null)
                    {
                        Console.Write("Enter Deposit Amount: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
                        {
                            depositAccount.Deposit(amount);
                            Console.WriteLine("\n[TRANSACTION SUCCESSFUL]");

                            // Step 13: Honor contract by executing PrintReceipt() post-transaction
                            depositAccount.PrintReceipt();
                        }
                        else Console.WriteLine("\n[ERROR] Invalid deposit numeric formatting.");
                    }
                    else Console.WriteLine("\n[ERROR] Account parameter registry location not found.");
                    Console.ReadKey();
                    break;

                case "4": // 4) Withdraw (Triggers Step 13 Receipt Confirmation)
                    Console.Clear();
                    Console.WriteLine("========================================");
                    Console.WriteLine("          FUNDS WITHDRAWAL ENGINE       ");
                    Console.WriteLine("========================================");
                    Console.Write("Enter Target Account Number: ");
                    string targetWithdrawNum = Console.ReadLine() ?? "";

                    Account? withdrawAccount = currentCustomer.Accounts.Find(a => a.AccountNumber.Equals(targetWithdrawNum, StringComparison.OrdinalIgnoreCase));
                    if (withdrawAccount != null)
                    {
                        Console.Write("Enter Withdrawal Amount: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal amount) && amount > 0)
                        {
                            // Dynamic polymorphism handles rules for savings vs checking limits automatically
                            withdrawAccount.Withdraw(amount);

                            Console.WriteLine("\n[TRANSACTION UPDATE]");
                            // Step 13: Confirming transaction updates via interface contract execution
                            withdrawAccount.PrintReceipt();
                        }
                        else Console.WriteLine("\n[ERROR] Invalid transaction numeric parsing allocation.");
                    }
                    else Console.WriteLine("\n[ERROR] Specified account link not found.");
                    Console.ReadKey();
                    break;

                case "5": // 5) Transfer (Triggers Step 13 Receipt Confirmation for Source)
                    Console.Clear();
                    Console.WriteLine("========================================");
                    Console.WriteLine("         SECURE INTER-BANK TRANSFER     ");
                    Console.WriteLine("========================================");
                    Console.Write("Enter SOURCE Account Number: ");
                    string srcNum = Console.ReadLine() ?? "";
                    Console.Write("Enter DESTINATION Account Number: ");
                    string destNum = Console.ReadLine() ?? "";

                    Account? srcAcc = currentCustomer.Accounts.Find(a => a.AccountNumber.Equals(srcNum, StringComparison.OrdinalIgnoreCase));
                    Account? destAcc = null;

                    foreach (var cust in customerDatabase)
                    {
                        destAcc = cust.Accounts.Find(a => a.AccountNumber.Equals(destNum, StringComparison.OrdinalIgnoreCase));
                        if (destAcc != null) break;
                    }

                    if (srcAcc != null && destAcc != null)
                    {
                        Console.Write("Enter Transfer Value: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal transferAmount) && transferAmount > 0)
                        {
                            decimal startingBalance = srcAcc.Balance;
                            srcAcc.Withdraw(transferAmount);

                            if (srcAcc.Balance < startingBalance) // If withdrawal logic cleared the verification phase
                            {
                                destAcc.Deposit(transferAmount);
                                Console.WriteLine("\n[TRANSFER COMPLETED SECURELY]");
                                Console.WriteLine("\n--- UPDATED DEBIT STATEMENT ---");

                                // Step 13: Interface proof on origin assets
                                srcAcc.PrintReceipt();
                            }
                        }
                    }
                    else Console.WriteLine("\n[ERROR] Core routing confirmation mapping failed.");
                    Console.ReadKey();
                    break;

                case "6": // 6) Close Account
                    Console.Clear();
                    Console.WriteLine("========================================");
                    Console.WriteLine("          ACCOUNT TERMINATION LINK      ");
                    Console.WriteLine("========================================");
                    Console.Write("Enter Account Number to close permanently: ");
                    string closeNum = Console.ReadLine() ?? "";

                    Account? targetToClose = currentCustomer.Accounts.Find(a => a.AccountNumber.Equals(closeNum, StringComparison.OrdinalIgnoreCase));
                    if (targetToClose != null)
                    {
                        currentCustomer.Accounts.Remove(targetToClose);
                        Console.WriteLine($"\n[CLOSED] Account container {closeNum} records successfully purged.");
                    }
                    else Console.WriteLine("\n[ERROR] Target array context not found.");
                    Console.ReadKey();
                    break;

                case "7": // 7) Exit
                    inDashboard = false;
                    break;
            }
        }
    }
}

public sealed record AddNumbersResponse(int A, int B, int Result);