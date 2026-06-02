using Microsoft.AspNetCore.Mvc;
using BankingAPI.Models;
using BankingAPI.Services;
using System.Linq;

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

        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetAllCustomers()
        {
            return Ok(_customerService.GetAllCustomers());
        }

        [HttpGet("{id}")]
        public ActionResult<Customer> GetCustomerById(int id)
        {
            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
                return NotFound(new { message = "Customer not found" });
            return Ok(customer);
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<Customer>> GetCustomerByName([FromQuery] string name)
        {
            return Ok(_customerService.GetCustomerByName(name));
        }

        [HttpGet("premium")]
        public ActionResult<IEnumerable<Customer>> GetAllPremiumCustomers([FromQuery] decimal? threshold)
        {
            return Ok(_customerService.GetAllPremiumCustomers(threshold));
        }

        [HttpPost]
        public ActionResult<Customer> CreateCustomer([FromBody] Customer customer)
        {
            var created = _customerService.CreateCustomer(customer);
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCustomer(int id, [FromBody] Customer update)
        {
            var existing = _customerService.UpdateCustomer(id, update);
            if (existing == null)
                return NotFound(new { message = "Customer not found" });

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCustomer(int id)
        {
            var deleted = _customerService.DeleteCustomer(id);
            if (!deleted)
                return NotFound(new { message = "Customer not found" });

            return NoContent();
        }
    }
}
