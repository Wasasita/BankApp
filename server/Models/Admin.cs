namespace Backend.Api.Models;

public class Admin : User
{
    public int Id { get; set; }

    public Admin(int id, string username, string password) : base(username, password)
    {
        Id = id;
    }
}