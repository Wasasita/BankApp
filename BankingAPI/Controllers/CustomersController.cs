using Microsoft.AspNetCore.Mvc;
using BankingAPI.Models;
using BankingAPI.Services;
using System.Collections.Generic;
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

        [HttpGet("{id}/total-balance")]
        public async Task<ActionResult> GetCustomerTotalBalance(int id)
        {
            // Added await
            var totalBalance = await _customerService.GetCustomerTotalBalance(id);

            if (totalBalance == null)
                return NotFound(new { message = "Customer not found" });

            return Ok(new { CustomerId = id, TotalBalance = totalBalance });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {
            // Added await
            var customers = await _customerService.GetAllCustomers();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(int id)
        {
            // Added await
            var customer = await _customerService.GetCustomerById(id);
            if (customer == null)
                return NotFound(new { message = "Customer not found" });
            return Ok(customer);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomerByName([FromQuery] string name)
        {
            // Added await
            var customers = await _customerService.GetCustomerByName(name);
            return Ok(customers);
        }

        [HttpGet("search-by-email")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomerByEmail([FromQuery] string email)
        {
            // Added await
            var customers = await _customerService.GetCustomerByEmail(email);
            return Ok(customers);
        }

        [HttpGet("premium")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllPremiumCustomers([FromQuery] decimal? threshold)
        {
            // Added await
            var customers = await _customerService.GetAllPremiumCustomers(threshold);
            return Ok(customers);
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer([FromBody] Customer customer)
        {
            // Added await
            var created = await _customerService.CreateCustomer(customer);

            if (created == null)
            {
                return BadRequest(new { message = "Customer with this email already exists"});
            }

            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCustomer(int id, [FromBody] Customer update)
        {
            // Added await
            var existing = await _customerService.UpdateCustomer(id, update);
            if (existing == null)
                return NotFound(new { message = "Customer not found" });

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            var deleted = await _customerService.DeleteCustomer(id);
            if (!deleted)
                return NotFound(new { message = "Customer not found" });

            return NoContent();
        }
    }
}