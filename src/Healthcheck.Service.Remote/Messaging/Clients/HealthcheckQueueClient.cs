namespace Healthcheck.Service.Remote.Messaging.Clients
{
    using Healthcheck.Service.Core;
    using Microsoft.Azure.ServiceBus;

    public class HealthcheckQueueClient : QueueClient
    {
        public HealthcheckQueueClient() : base(SharedConfig.ConnectionStringOrKey, SharedConfig.IncomingQueueName)
        {
        }
    }
}