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
            RestClient c = new RestClient("http://customerapi/customer/");

            var request = new RestRequest(id.ToString());
            var response = c.Execute<CustomerDto>(request);
            if (response.Data != null)
            {


                return response.Data;

            }

            else
            {
                return null;
            }
        }
    }
}
