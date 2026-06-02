using Microsoft.AspNetCore.Mvc;
using BankingAPI.Models;
using BankingAPI.Services;
using System.Linq;

namespace BankingAPI.Controllers
{ 
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly AccountService _accountService;

        public AccountsController(AccountService accountService)
        {
            _accountService = accountService;
        }

        //Step 4: Implement GET (HttpGet) Endpoints (Read):
        // Start with simple endpoints like GetAllCustomers and GetCustomerById.
        [HttpGet]
        public ActionResult<IEnumerable<Account>> GetAllAccounts()
        {
            return Ok(_accountService.GetAllAccounts());
        }

        [HttpGet("{id}")]
        public ActionResult<Account> GetAccountById(int id)
        {
            var account = _accountService.GetAccountById(id);
            if (account == null)
                return NotFound(new { message = "Account not found" });
            return Ok(account);
        }

        // api/accounts/search?name={name}
        [HttpGet("search")]
        public ActionResult<IEnumerable<Account>> GetAccountByName([FromQuery] string name)
        {
            return Ok(_accountService.GetAccountByName(name));
        }

        //Step 6: Implement POST (HttpPost), PUT(HttpPut), and DELETE(HttpDelete) Endpoints (Write/Mutate): 
        [HttpPost]
        public ActionResult<Account> CreateAccount([FromQuery] int customerId, [FromBody] Account account)
        {
            var created = _accountService.CreateAccount(customerId, account);
            if (created == null)
                return NotFound(new { message = "Customer not found" });

            return CreatedAtAction(nameof(GetAccountById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateAccount(int id, [FromBody] Account update)
        {
            var existing = _accountService.UpdateAccount(id, update);
            if (existing == null)
                return NotFound(new { message = "Account not found" });

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteAccount(int id)
        {
            var deleted = _accountService.DeleteAccount(id);
            if (!deleted)
                return NotFound(new { message = "Account not found" });

            return NoContent();
        }
    }
}
