namespace BankingAPI.Data;

public class JwtSettings
{
    public string Issuer { get; set; } = "BankingAPI";
    public string Audience { get; set; } = "BankingAPI.Frontend";
    public string SecretKey { get; set; } = string.Empty;
    public int ExpirationMinutes { get; set; } = 60;
}
