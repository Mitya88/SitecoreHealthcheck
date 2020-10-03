using Healthcheck.Service.Core;
using Microsoft.Azure.ServiceBus;

namespace Healthcheck.Service.Remote.Messaging.Clients
{
    public class HealthcheckSubscriptionClient : SubscriptionClient
    {
        public HealthcheckSubscriptionClient() : base(SharedConfig.ConnectionStringOrKey, SharedConfig.TopicName, SharedConfig.SubscriptionName)
        {
        }
    }
}