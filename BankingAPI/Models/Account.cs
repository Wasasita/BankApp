using MongoDB.Bson.Serialization.Attributes;

namespace BankingAPI.Models
{
    [BsonIgnoreExtraElements]
    public class Account
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string AccountNumber { get; set; } = string.Empty;

        public string AccountType { get; set; } = string.Empty;

        public decimal Balance { get; set; }
    }
}