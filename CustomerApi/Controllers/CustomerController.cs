using CustomerApi.Data;
using CustomerApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomerApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private IRepository<Customer> _repo;
        public CustomerController(IRepository<Customer> repo) 
        {
            _repo = repo;
        }
        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return _repo.GetAll();
        }
        //get by customerId
        [Route("GetCustomer/{id}")]
        public IActionResult GetCustomer(int id) 
        {
            var item = _repo.Get(id);
            if (item == null) 
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }
        //add customer
        [HttpPost]
        public IActionResult Post([FromBody] Customer customer) 
        {
            if (customer == null)
            {
                return BadRequest();
            }

            var newCustomer = _repo.Add(customer);
            return CreatedAtRoute("GetCustomer", new { id = newCustomer.Id }, newCustomer);
        }
        //delete customer
        [HttpDelete("{id}")]
        public IActionResult Delete(int id) 
        {
            if (id == 0)
            {
                return BadRequest();
            }

            _repo.Remove(id);
            return Ok();
        }
        //update customer
        [HttpPut]
        public IActionResult Update([FromBody] Customer customer) 
        {
            if (customer == null) 
            {
                return BadRequest();
            }
            _repo.Edit(customer);
            return Ok();
        }

        
        
        
        
    }
}
