using BankingAPI.Data;
using BankingAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BankingAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var db = client.GetDatabase(settings.Value.DatabaseName);
        _users = db.GetCollection<User>(settings.Value.UsersCollectionName);
    }

    public async Task<User?> GetByUsernameAsync(string username)
        => await _users.Find(u => u.Username == username).FirstOrDefaultAsync();

    public async Task<User> CreateAsync(User user)
    {
        var last = await _users.Find(_ => true)
            .SortByDescending(u => u.Id)
            .FirstOrDefaultAsync();

        user.Id = last == null ? 1 : last.Id + 1;

        await _users.InsertOneAsync(user);
        return user;
    }
}
