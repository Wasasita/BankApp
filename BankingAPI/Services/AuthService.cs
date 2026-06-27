using BankingAPI.Data;
using BankingAPI.Models;
using BankingAPI.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BankingAPI.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUserRepository userRepository, IOptions<JwtSettings> jwtOptions)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var loginIdentifier = request.GetLoginIdentifier();
        if (string.IsNullOrWhiteSpace(loginIdentifier) || string.IsNullOrWhiteSpace(request.Password))
        {
            return null;
        }

        var user = await _userRepository.GetByUsernameAsync(loginIdentifier);
        if (user == null)
        {
            return null;
        }

        var requestPasswordHash = ComputeSha256Hash(request.Password);
        if (!string.Equals(user.PasswordHash, requestPasswordHash, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var token = CreateJwtToken(user);
        return new LoginResponse { Token = token };
    }

    public async Task<(SignupResponse? Response, string? Error)> SignupAsync(SignupRequest request)
    {
        var username = request.GetUsername();
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return (null, "Username (or email) and password are required");
        }

        var role = string.IsNullOrWhiteSpace(request.Role) ? "Admin" : request.Role.Trim();
        if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(role, "User", StringComparison.OrdinalIgnoreCase))
        {
            return (null, "Role must be Admin or User");
        }

        var existingUser = await _userRepository.GetByUsernameAsync(username);
        if (existingUser != null)
        {
            return (null, "A user with this username already exists");
        }

        var user = new User
        {
            Username = username,
            PasswordHash = ComputeSha256Hash(request.Password),
            Role = char.ToUpper(role[0]) + role[1..].ToLower()
        };

        var createdUser = await _userRepository.CreateAsync(user);
        return (new SignupResponse
        {
            Id = createdUser.Id,
            Username = createdUser.Username,
            Role = createdUser.Role
        }, null);
    }

    private string CreateJwtToken(User user)
    {
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            SecurityAlgorithms.HmacSha256
        );

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string ComputeSha256Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}
