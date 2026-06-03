using Microsoft.AspNetCore.Mvc;
using BankingAPI.Models;
using BankingAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankingAPI.Controllers
{ 
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly CustomerService _customerService;

        public AccountsController(AccountService accountService, CustomerService customerService)
        {
            _accountService = accountService;
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Customer>>> GetAllCustomers()
        {
            var customers = await _customerService.GetAllCustomers(); 
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccountById(int id)
        {
            // Added await
            var account = await _accountService.GetAccountById(id);
            if (account == null)
                return NotFound(new { message = "Account not found" });
            return Ok(account);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Account>>> GetAccountByName([FromQuery] string name)
        {
            // Added await
            var accounts = await _accountService.GetAccountByName(name);
            return Ok(accounts);
        }

        [HttpPost]
        public async Task<ActionResult<Account>> CreateAccount([FromQuery] int customerId, [FromBody] Account account)
        {
            // Added await
            var created = await _accountService.CreateAccount(customerId, account);
            if (created == null)
                return NotFound(new { message = "Customer not found" });

            // Note: Fixed created.Id routing mapping compilation constraint
            return CreatedAtAction(nameof(GetAccountById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAccount(int id, [FromBody] Account update)
        {
            // Added await
            var existing = await _accountService.UpdateAccount(id, update);
            if (existing == null)
                return NotFound(new { message = "Account not found" });

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAccount(int id)
        {
            var deleted = await _accountService.DeleteAccount(id);

            if (!deleted)
                return NotFound(new { message = "Account not found" });

            return NoContent();
        }
    }
}