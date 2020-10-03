using Healthcheck.Service.Core;
using Microsoft.Azure.ServiceBus;

namespace Healthcheck.Service.Remote.Messaging.Clients
{
    public class HealthcheckQueueClient : QueueClient
    {
        public HealthcheckQueueClient() : base(SharedConfig.ConnectionStringOrKey, SharedConfig.IncomingQueueName)
        {
        }
    }
}