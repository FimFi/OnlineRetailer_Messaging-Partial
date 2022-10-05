using RestSharp;
using SharedModels;

namespace OrderApi.Infrastructure
{
    public class CustomerServiceGateway : IServiceGateway<CustomerDto>
    {
        string customerServiceBaseUrl;

        public CustomerServiceGateway(string baseUrl)
        {
            customerServiceBaseUrl = baseUrl;
        }

        public CustomerDto Get(int id)
        {
            RestClient c = new RestClient(customerServiceBaseUrl);

            var request = new RestRequest(id.ToString());
            var response = c.Execute<CustomerDto>(request);
            
            return response.Data;
        }
    }
}
