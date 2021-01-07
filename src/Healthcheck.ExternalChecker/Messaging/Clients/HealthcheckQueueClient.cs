namespace Healthcheck.ExternalChecker
{
    using Healthcheck.Service.Core;
    using Microsoft.Azure.ServiceBus;
    using System.Configuration;

    public class HealthcheckQueueClient : QueueClient
    {
        public HealthcheckQueueClient() : base(ConfigurationManager.ConnectionStrings["sb"].ConnectionString, 
            ConfigurationManager.AppSettings["Healthcheck.IncomingQueueName"])
        {
        }
    }
}