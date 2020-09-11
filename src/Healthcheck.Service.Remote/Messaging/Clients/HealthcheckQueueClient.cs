using Healthcheck.Service.Core;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcheck.Service.Remote.Messaging.Clients
{
    public class HealthcheckQueueClient :QueueClient
    {
        public HealthcheckQueueClient():base(SharedConfig.ConnectionStringOrKey,SharedConfig.IncomingQueueName)
        {
        }
    }
}
