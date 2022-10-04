using System;
using System.Collections.Generic;
using EasyNetQ;
using SharedModels;

namespace OrderApi.Infrastructure
{
    public class MessagePublisher : IMessagePublisher, IDisposable
    {
        IBus bus;

        public MessagePublisher(string connectionString)
        {
            bus = RabbitHutch.CreateBus(connectionString);
        }

        public void Dispose()
        {
            bus.Dispose();
        }

        public void PublishOrderStatusChangedMessage(int customerId, IList<OrderLine> orderLines, string topic)
        {
            var message = new OrderStatusChangedMessage
            { 
                CustomerId = customerId,
                OrderLines = orderLines 
            };

            bus.PubSub.Publish(message, topic);
        }

        public async Task<CustomerDto> RequestCustomer(int id) 
        {

            var request = new RequestedCustomer { CustomerId = id };
            var response = await bus.Rpc.RequestAsync<RequestedCustomer, CustomerDto>(request);
            return response;
        }
    }
}
