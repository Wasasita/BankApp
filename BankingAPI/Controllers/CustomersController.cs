using Microsoft.AspNetCore.Mvc;
using BankingAPI.Data;
using BankingAPI.Models;
using System.Linq;

namespace BankingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        // Same ones as Account Controller 
        [HttpGet]
        public ActionResult<IEnumerable<Customer>> GetAllCustomers()
        {
            return Ok(DataStore.Customers);
        }

        [HttpGet("{id}")]
        public ActionResult<Customer> GetCustomerById(int id)
        {
            var customer = DataStore.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null) // if ID does not exist, API responds with a standard 404 Not Found HTTP instead of a generic server crash.
                return NotFound(new { message = "Customer not found" });
            return Ok(customer);
        }

        //Step 5: Implement Filtered Logic (Premium Customers): For GetAllPremiumCustomers, implement
        // a filtering mechanism using loops or query expressions (e.g., LINQ, Streams, or array filters) to
        // calculate if the balance exceeds your defined numeric threshold (e.g., $10,000)
        [HttpGet("search")]
        public ActionResult<IEnumerable<Customer>> GetCustomerByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Ok(DataStore.Customers);

            var matches = DataStore.Customers
                .Where(c => c.Name.Contains(name, System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(matches);
        }

        [HttpGet("premium")]
        public ActionResult<IEnumerable<Customer>> GetAllPremiumCustomers([FromQuery] decimal? threshold)
        {
            decimal t = threshold ?? 10000m;
            var premium = DataStore.Customers
                .Where(c => c.Accounts.Sum(a => a.Balance) > t)
                .ToList();

            return Ok(premium);
        }

        [HttpPost]
        public ActionResult<Customer> CreateCustomer([FromBody] Customer customer)
        {
            var newId = DataStore.Customers.Any() ? DataStore.Customers.Max(c => c.Id) + 1 : 1;
            customer.Id = newId;
            customer.Accounts = customer.Accounts ?? new List<Account>();
            DataStore.Customers.Add(customer);
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCustomer(int id, [FromBody] Customer update)
        {
            var existing = DataStore.Customers.FirstOrDefault(c => c.Id == id);
            if (existing == null)
                return NotFound(new { message = "Customer not found" });

            existing.Name = update.Name;
            existing.Email = update.Email;
            // keep accounts in sync only via account endpoints

            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCustomer(int id)
        {
            var customer = DataStore.Customers.FirstOrDefault(c => c.Id == id);
            if (customer == null)
                return NotFound(new { message = "Customer not found" });

            var accountIds = customer.Accounts.Select(a => a.Id).ToList();
            DataStore.Accounts.RemoveAll(a => accountIds.Contains(a.Id));
            customer.Accounts.Clear();
            DataStore.Customers.Remove(customer);

            return NoContent();
        }
    }
}
