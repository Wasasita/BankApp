using Microsoft.AspNetCore.Mvc;
using BankingAPI.Models;
using BankingAPI.Services;
using System.Linq;
using System.Threading.Tasks;

namespace BankingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerService _customerService;

        public CustomersController(CustomerService customerService)
        {
            _customerService = customerService;
        }

        // this is an endpoint I added myself called total-balance that calculates the total balance of all accounts for a given customer
        [HttpGet("{id}/total-balance")]
        public async Task<ActionResult> GetCustomerTotalBalance(int id)
        {
            var totalBalance = await _customerService.GetCustomerTotalBalanceAsync(id);

            if (totalBalance == null)
                return NotFound(new { message = "Customer not found" });

            return Ok(new { CustomerId = id, TotalBalance = totalBalance });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {
            return Ok(await _customerService.GetAllCustomersAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
                return NotFound(new { message = "Customer not found" });
            return Ok(customer);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomerByName([FromQuery] string name)
        {
            return Ok(await _customerService.GetCustomerByNameAsync(name));
        }

        [HttpGet("search-by-email")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomerByEmail([FromQuery] string email)
        {
            return Ok(await _customerService.GetCustomerByEmailAsync(email));
        }

        [HttpGet("premium")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllPremiumCustomers([FromQuery] decimal? threshold)
        {
            return Ok(await _customerService.GetAllPremiumCustomersAsync(threshold));
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer([FromBody] Customer customer)
        {
            var created = await _customerService.CreateCustomerAsync(customer);

            if (created == null){
                return BadRequest(new { message = "Customer with this email already exists"});
            }

            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCustomer(int id, [FromBody] Customer update)
        {
            var existing = await _customerService.UpdateCustomerAsync(id, update);
            if (existing == null)
                return NotFound(new { message = "Customer not found" });

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            var deleted = await _customerService.DeleteCustomerAsync(id);
            if (!deleted)
                return NotFound(new { message = "Customer not found" });

            return NoContent();
        }
    }
}
