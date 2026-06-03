using MongoDB.Bson.Serialization.Attributes;

namespace BankingAPI.Models
{
    [BsonIgnoreExtraElements]
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public List<Account> Accounts { get; set; } = new();
    }
}