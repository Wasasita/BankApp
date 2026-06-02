using System.Collections.Generic;

namespace BankingAPI.Models
{
    // Step2: Customer Model: Should include fields like Id, Name, Email, and a collection/list of associated Accounts
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public List<Account> Accounts { get; set; } = new List<Account>();
    }
}
