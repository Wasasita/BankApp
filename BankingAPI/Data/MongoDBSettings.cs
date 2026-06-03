namespace BankingAPI.Data;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string CustomersCollectionName { get; set; } = "Customers";
    public string AccountsCollectionName { get; set; } = "Accounts";
}