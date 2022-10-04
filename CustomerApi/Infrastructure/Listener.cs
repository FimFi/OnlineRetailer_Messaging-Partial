using CustomerApi.Data;
using CustomerApi.Models;
using EasyNetQ;
using SharedModels;

namespace CustomerApi.Infrastructure
{
    public class Listener
    {
        IServiceProvider _provider;
        string _connectionString;
        IBus bus;

        public Listener(IServiceProvider provider, string connectionString)
        {
            _provider = provider;
            _connectionString = connectionString;
        }

        public void Start()
        {
            using (bus = RabbitHutch.CreateBus(_connectionString))
            {
                bus.PubSub.Subscribe<CreateOrderMessage>("customerApiCreated", HandleOrderCreated);
                bus.PubSub.Subscribe<PayOrderMessage>("customerApiPay", HandleOrderPaid);

                lock (this)
                {
                    Monitor.Wait(this);
                }
            }
        }

        private void HandleOrderPaid(PayOrderMessage message)
        {
            using (var scope = _provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var customerRepos = services.GetService<IRepository<Customer>>();

                if (!CustomerHasGoodStanding(message.CustomerId, customerRepos))
                {
                    var customer = customerRepos.Get(message.CustomerId);
                    customer.CreditStanding = true;
                    customerRepos.Edit(customer);

                    var replyMessage = new AcceptedPaidOrderMessage
                    {
                        OrderId = message.OrderId
                    };

                    bus.PubSub.Publish(replyMessage);
                }
                else
                {
                    var replyMessage = new RejectOrderPaidMessage
                    {
                        OrderId = message.OrderId
                    };

                    bus.PubSub.Publish(replyMessage);
                }
            }
        }

        private void HandleOrderCreated(CreateOrderMessage message)
        {
            using (var scope = _provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var customerRepos = services.GetService<IRepository<Customer>>();

                if (CustomerHasGoodStanding(message.CustomerId, customerRepos))
                {
                    var customer = customerRepos.Get(message.CustomerId);
                    customer.CreditStanding = false;
                    customerRepos.Edit(customer);

                    var replyMessage = new AcceptedOrderMessage
                    {
                        OrderId = message.OrderId
                    };

                    bus.PubSub.Publish(replyMessage);
                }
                else
                {
                    var replyMessage = new RejectOrderMessage
                    {
                        OrderId = message.OrderId
                    };

                    bus.PubSub.Publish(replyMessage);
                }
            }
        }

        private bool CustomerHasGoodStanding(int customerId, IRepository<Customer> customerRepos)
        {
            var customer = customerRepos.Get(customerId);
            if (customer != null && customer.CreditStanding)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
