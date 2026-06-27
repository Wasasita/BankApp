namespace BankingAPI.Models;

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string GetLoginIdentifier()
    {
        if (!string.IsNullOrWhiteSpace(Username))
        {
            return Username.Trim();
        }

        if (!string.IsNullOrWhiteSpace(Email))
        {
            return Email.Trim();
        }

        return string.Empty;
    }
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
}

public class SignupRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "Admin";

    public string GetUsername()
    {
        if (!string.IsNullOrWhiteSpace(Username))
        {
            return Username.Trim();
        }

        if (!string.IsNullOrWhiteSpace(Email))
        {
            return Email.Trim();
        }

        return string.Empty;
    }
}

public class SignupResponse
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
