using BankingAPI.Models;
using BankingAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

        [HttpGet]
        public async Task<ActionResult<List<AccountResponse>>> GetAllAccounts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var accounts = await _accountService.GetAllAccountsAsync();
            var paged = accounts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(paged);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AccountResponse>> GetAccountById(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null)
                return NotFound(new { message = "Account not found" });

            return Ok(account);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<AccountResponse>>> SearchAccounts([FromQuery] string? name, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var accounts = await _accountService.SearchAccountsByCustomerNameAsync(name);
            var paged = accounts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(paged);
        }

        [HttpPost]
        public async Task<ActionResult<Account>> CreateAccount([FromQuery] int customerId, [FromBody] Account account)
        {
            var created = await _accountService.CreateAccountAsync(customerId, account);
            if (created == null)
                return NotFound(new { message = "Customer not found" });

            return CreatedAtAction(nameof(GetAccountById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Account>> UpdateAccount(int id, [FromBody] Account update)
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