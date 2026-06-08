using MongoDB.Bson.Serialization.Attributes;

namespace BankingAPI.Models;

[BsonIgnoreExtraElements]
public class Transaction
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Description { get; set; }
}
