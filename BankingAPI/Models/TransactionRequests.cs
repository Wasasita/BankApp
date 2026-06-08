namespace BankingAPI.Models;

public class DepositRequest
{
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}

public class WithdrawRequest
{
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}

public class TransferRequest
{
    public int FromAccountId { get; set; }
    public int ToAccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
}
