namespace BankingAPI.Data;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string CustomersCollectionName { get; set; } = null!;

    public string AccountsCollectionName { get; set; } = null!;
}