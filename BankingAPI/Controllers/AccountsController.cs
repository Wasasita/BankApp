using Microsoft.AspNetCore.Mvc;
using BankingAPI.Data;
using BankingAPI.Models;
using System.Linq;

namespace BankingAPI.Controllers
{ 
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        //Step 4: Implement GET (HttpGet) Endpoints (Read):
        // Start with simple endpoints like GetAllCustomers and GetCustomerById.
        [HttpGet]
        public ActionResult<IEnumerable<Account>> GetAllAccounts()
        {
            return Ok(DataStore.Accounts);
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccountById(int id)
        {
            var account = DataStore.Accounts.FirstOrDefault(a => a.Id == id);
            if (account == null) // if ID does not exist, API responds with a standard 404 Not Found HTTP instead of a generic server crash.
                return NotFound(new { message = "Account not found" });
            return Ok(account);
        }

        // api/accounts/search?name={name}
        [HttpGet("search")]
        public ActionResult<IEnumerable<Account>> GetAccountByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Ok(DataStore.Accounts);

            var accounts = DataStore.Customers
                .Where(c => c.Name.Contains(name, System.StringComparison.OrdinalIgnoreCase))
                .SelectMany(c => c.Accounts)
                .ToList();

            return Ok(accounts);
        }

        //Step 6: Implement POST (HttpPost), PUT(HttpPut), and DELETE(HttpDelete) Endpoints (Write/Mutate): 
        [HttpPost]
        public ActionResult<Account> CreateAccount([FromQuery] int customerId, [FromBody] Account account)
        {
            var customer = DataStore.Customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null)
                return NotFound(new { message = "Customer not found" });

            // Ensure you assign a new unique identifier (ID) when a new item is pushed into the collection.
            var newId = DataStore.Accounts.Any() ? DataStore.Accounts.Max(a => a.Id) + 1 : 1;
            account.Id = newId;

            DataStore.Accounts.Add(account);
            customer.Accounts.Add(account);

            return CreatedAtAction(nameof(GetAccountById), new { id = account.Id }, account);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateAccount(int id, [FromBody] Account update)
        {
            var existing = DataStore.Accounts.FirstOrDefault(a => a.Id == id);
            if (existing == null)
                return NotFound(new { message = "Account not found" });

            existing.AccountNumber = update.AccountNumber;
            existing.AccountType = update.AccountType;
            existing.Balance = update.Balance;

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteAccount(int id)
        {
            var acc = DataStore.Accounts.FirstOrDefault(a => a.Id == id);
            if (acc == null)
                return NotFound(new { message = "Account not found" });

            DataStore.Accounts.Remove(acc);
            foreach (var c in DataStore.Customers)
                c.Accounts.RemoveAll(a => a.Id == id);

            return NoContent();
        }
    }
}
