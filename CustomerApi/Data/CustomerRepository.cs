using CustomerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Data
{
    public class CustomerRepository : IRepository<Customer>
    {
        private CustomerApiContext _context;
        public CustomerRepository(CustomerApiContext context)
        {
            _context = context;
        }
        Customer IRepository<Customer>.Add(Customer entity)
        {
            var newCustomer = _context.Customers.Add(entity).Entity;
            _context.SaveChanges();
            return newCustomer;
        }

        public void Edit(Customer entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public Customer Get(int id)
        {
            return _context.Customers.FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<Customer> GetAll()
        {
            return _context.Customers.ToList();
        }

        public void Remove(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == id);
            _context.Customers.Remove(customer);
            _context.SaveChanges();
        }
    }
}
