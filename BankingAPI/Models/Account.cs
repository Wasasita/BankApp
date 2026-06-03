using MongoDB.Bson.Serialization.Attributes;

namespace BankingAPI.Models
{
    [BsonIgnoreExtraElements]
    // Step 2: Define Data Models: Account Model: Should include fields like Id, AccountNumber, AccountType (Savings,Checking),and Balance.
    public class Account
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public string AccountNumber { get; set; } = string.Empty;

        public string AccountType { get; set; } = string.Empty;

        public decimal Balance { get; set; }
    }
}
