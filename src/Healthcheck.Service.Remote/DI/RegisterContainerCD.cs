namespace Healthcheck.Service.Remote.DI
{
   
    using Healthcheck.Service.Remote.Messaging.Clients;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.DependencyInjection;

    /// <summary>
    /// Registering the DI container
    /// </summary>
    /// <seealso cref="Sitecore.DependencyInjection.IServicesConfigurator"/>
    public class RegisterContainerCD : IServicesConfigurator
    {
        /// <summary>
        /// Configures the specified service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ISubscriptionClient, HealthcheckSubscriptionClient>();
        }
    }
}