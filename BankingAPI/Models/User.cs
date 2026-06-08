using MongoDB.Bson.Serialization.Attributes;

namespace BankingAPI.Models;

[BsonIgnoreExtraElements]
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
