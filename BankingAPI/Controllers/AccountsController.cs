using Microsoft.AspNetCore.Mvc;
using BankingAPI.Models;
using BankingAPI.Services;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<ActionResult<IEnumerable<Account>>> GetAllAccounts()
        {
            return Ok(await _accountService.GetAllAccountsAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccountById(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
                return NotFound(new { message = "Account not found" });
            return Ok(account);
        }

        // api/accounts/search?name={name}
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccountByName([FromQuery] string name)
        {
            return Ok(await _accountService.GetAccountByNameAsync(name));
        }

        //Step 6: Implement POST (HttpPost), PUT(HttpPut), and DELETE(HttpDelete) Endpoints (Write/Mutate): 
        [HttpPost]
        public async Task<ActionResult<Account>> CreateAccount([FromQuery] int customerId, [FromBody] Account account)
        {
            var created = await _accountService.CreateAccountAsync(customerId, account);
            if (created == null)
                return NotFound(new { message = "Customer not found" });

            return CreatedAtAction(nameof(GetAccountById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAccount(int id, [FromBody] Account update)
        {
            var existing = await _accountService.UpdateAccountAsync(id, update);
            if (existing == null)
                return NotFound(new { message = "Account not found" });

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAccount(int id)
        {
            var deleted = await _accountService.DeleteAccountAsync(id);
            if (!deleted)
                return NotFound(new { message = "Account not found" });

            return NoContent();
        }
    }
}
