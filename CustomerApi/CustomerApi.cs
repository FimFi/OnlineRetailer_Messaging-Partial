namespace CustomerApi
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.IO;
    using System.Net.Http;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.ServiceFabric;
    using Microsoft.ServiceFabric.Data;
    public class CustomerApi : StatefulService
    {
        public CustomerApi(StatefulServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        /// protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[]
            {
                new ServiceReplicaListener(
                    serviceContext =>
                        new KestrelCommunicationListener(
                            serviceContext,
                            (url, listener) =>
                            {
                                ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

                                return new WebHostBuilder()
                                    .UseKestrel()
                                    .ConfigureLogging(logging =>
                                    {
                                        logging.ClearProviders();
                                        logging.AddApplicationInsights();
                                    })
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<StatefulServiceContext>(serviceContext)
                                            .AddSingleton<IReliableStateManager>(this.StateManager)
                                            .AddSingleton<ITelemetryInitializer>((serviceProvider) => FabricTelemetryInitializerExtension.CreateFabricTelemetryInitializer(serviceContext)))
                                        .UseContentRoot(Directory.GetCurrentDirectory())
                                        .UseStartup<Startup>()
                                        .UseApplicationInsights()
                                        .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.UseUniqueServiceUrl)
                                        .UseUrls(url)
                                        .Build();

                            }))
            };
        }




        /// <summary>
        /// Constructs a service name for a specific poll.
        /// Example: fabric:/VotingApplication/polls/name-of-poll
        /// </summary>
        /// <param name="poll"></param>
        /// <returns></returns>
        internal static Uri GetCustomerApiServiceName(ServiceContext context)
        {
            return new Uri($"{context.CodePackageActivationContext.ApplicationName}/CustomerData");
        }
    }
}
