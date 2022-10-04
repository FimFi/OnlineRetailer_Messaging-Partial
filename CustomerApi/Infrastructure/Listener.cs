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

                lock (this)
                {
                    Monitor.Wait(this);
                }
            }
        }

    }
}
