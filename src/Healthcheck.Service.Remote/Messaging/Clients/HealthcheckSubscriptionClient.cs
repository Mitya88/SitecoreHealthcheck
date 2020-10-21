namespace Healthcheck.Service.Remote.Messaging.Clients
{
    using Healthcheck.Service.Core;
    using Microsoft.Azure.ServiceBus;

    public class HealthcheckSubscriptionClient : SubscriptionClient
    {
        public HealthcheckSubscriptionClient() : base(SharedConfig.ConnectionStringOrKey, SharedConfig.TopicName, SharedConfig.SubscriptionName)
        {
        }
    }
}