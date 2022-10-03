using CustomerApi.Models;

namespace CustomerApi.Data
{
    public class DbInitializer : IDbInitializer
    {
        // This method will create and seed the database.
        public void Initialize(CustomerApiContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Look for any Customers
            if (context.Customers.Any())
            {
                return;   // DB has been seeded
            }

            List<Customer> customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Jens", Email = "test@gmail.com", Phone = 12313523, BillingAddress = "Address", ShippingAddress = "Address", CreditStanding = false},
                new Customer { Id = 2, Name = "Kurt", Email = "test2@gmail.com", Phone = 22512323, BillingAddress = "Address1", ShippingAddress = "Address1", CreditStanding = true}
            };

            context.Customers.AddRange(customers);
            context.SaveChanges();
        }
    }
}
