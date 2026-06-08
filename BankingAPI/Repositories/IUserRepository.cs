using BankingAPI.Models;

namespace BankingAPI.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
}
