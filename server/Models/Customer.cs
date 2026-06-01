namespace Backend.Api.Models;

public class Customer : User
{
    // 1. Explicit private fields (Encapsulation hidden from the outside)
    private int _id;
    private string _firstName;
    private string _lastName;

    // 2. Public properties exposed to the outside world
    public int Id 
    { 
        get => _id; 
        set => _id = value; 
    }
    
    public string FirstName 
    { 
        get => _firstName; 
        set => _firstName = value; 
    }
    
    public string LastName 
    { 
        get => _lastName; 
        set => _lastName = value; 
    }
    
    public List<Account> Accounts { get; set; } = new List<Account>();

    public Customer(int id, string firstName, string lastName, string username, string password) 
        : base(username, password)
    {
        _id = id;
        _firstName = firstName;
        _lastName = lastName;
    }

    public override string ToString()
    {
        return $"Customer ID: {Id}, Name: {FirstName} {LastName}, Total Accounts: {Accounts.Count}";
    }
}