namespace Healthcheck.Service.DI
{
    using Healthcheck.Service.Controllers;
    using Healthcheck.Service.Factories;
    using Healthcheck.Service.Interfaces;
    using Healthcheck.Service.Repositories;
    using Healthcheck.Service.Services;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.DependencyInjection;

    /// <summary>
    /// Registering the DI container
    /// </summary>
    /// <seealso cref="Sitecore.DependencyInjection.IServicesConfigurator"/>
    public class RegisterContainer : IServicesConfigurator
    {
        /// <summary>
        /// Configures the specified service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection.</param>
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IHealthcheckRepository, HealthcheckRepository>();
            serviceCollection.AddTransient<IHealthcheckService, HealthcheckService>();
            serviceCollection.AddTransient<IComponentFactory, ComponentFactory>();
            serviceCollection.AddTransient<IApplicationInsightsService, ApplicationInsightsService>();

            serviceCollection.AddTransient<HealthcheckApiController>();
            serviceCollection.AddTransient<HealthcheckErrorsApiController>();
        }
    }
}