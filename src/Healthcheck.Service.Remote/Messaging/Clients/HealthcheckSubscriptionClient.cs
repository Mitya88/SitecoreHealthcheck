using Healthcheck.Service.Core;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcheck.Service.Remote.Messaging.Clients
{
    public class HealthcheckSubscriptionClient: SubscriptionClient
    {
        public HealthcheckSubscriptionClient():base(SharedConfig.ConnectionStringOrKey,SharedConfig.TopicName, SharedConfig.SubscriptionName)
        {

        }
    }
}
