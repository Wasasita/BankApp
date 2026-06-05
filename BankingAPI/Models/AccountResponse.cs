namespace BankingAPI.Models
{
    public class AccountResponse
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string CustomerName { get; set; } = string.Empty;
    }
}