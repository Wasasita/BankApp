using Backend.Api.Models;
using Backend.Api.Repositories;
using Backend.Api.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

List<Customer> customers = SeedCustomers();

builder.Services.AddControllers();
builder.Services.AddSingleton<ICustomerRepository>(new CustomerRepository(customers));
builder.Services.AddScoped<ICustomerService, CustomerService>();

WebApplication app = builder.Build();

app.MapControllers();
app.Run();

static List<Customer> SeedCustomers()
{
    int customerCounter = 1;
    int accountCounter = 1001;
    List<Customer> customers = new List<Customer>();

    Customer customerOne = new Customer(customerCounter++, "John", "Doe", "john_doe", "secure456");
    Customer customerTwo = new Customer(customerCounter++, "Jane", "Smith", "jane_smith", "password789");

    CheckingAccount checkingOne = new CheckingAccount($"CH-{accountCounter++}", customerOne, 250.00m, 100.00m);
    SavingsAccount savingsOne = new SavingsAccount($"SV-{accountCounter++}", customerOne, 1500.00m, 0.02m);
    customerOne.Accounts.Add(checkingOne);
    customerOne.Accounts.Add(savingsOne);

    CheckingAccount checkingTwo = new CheckingAccount($"CH-{accountCounter++}", customerTwo, 50.00m, 50.00m);
    customerTwo.Accounts.Add(checkingTwo);

    customers.Add(customerOne);
    customers.Add(customerTwo);

    return customers;
}
