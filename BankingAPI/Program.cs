using BankingAPI.Data;
using BankingAPI.Repositories;
using BankingAPI.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// // Load env (optional)
// DotNetEnv.Env.Load();
// // 2. Tell .NET Configuration to pull from those environment variables
// builder.Configuration.AddEnvironmentVariables();

// Mongo settings
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings")
);

// Mongo client singleton
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;

    if (string.IsNullOrEmpty(settings.ConnectionString))
    {
        throw new Exception("MongoDB connection string is missing");
    }

    return new MongoClient(settings.ConnectionString);
});

// Repositories
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

// Services
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<AccountService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173", // Vite dev server (IMPORTANT)
                "https://bank-app-eight.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("Frontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();