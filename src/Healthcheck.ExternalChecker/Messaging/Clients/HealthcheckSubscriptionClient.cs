namespace Healthcheck.ExternalChecker
{

    using Microsoft.Azure.ServiceBus;
    using System.Configuration;

    public class HealthcheckSubscriptionClient : SubscriptionClient
    {
        public HealthcheckSubscriptionClient() : base(
            ConfigurationManager.ConnectionStrings["sb"].ConnectionString, 
            ConfigurationManager.AppSettings["Healthcheck.TopicName"],
            ConfigurationManager.AppSettings["Healthcheck.SubscritionName"])
        {
        }
    }
}