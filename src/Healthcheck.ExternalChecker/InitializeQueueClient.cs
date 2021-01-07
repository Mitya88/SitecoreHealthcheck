namespace Healthcheck.ExternalChecker
{
    using Healthcheck.Service.Core;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;
    using System;
    using System.Configuration;

    public class InitializeQueueClient
    {
        public static void Init()
        {
            var _managementClient = new ManagementClient(new ServiceBusConnectionStringBuilder(ConfigurationManager.ConnectionStrings["sb"].ConnectionString));
            EnsureQueueExists(_managementClient, ConfigurationManager.AppSettings["Healthcheck.IncomingQueueName"]);
        }

        private static QueueDescription EnsureQueueExists(ManagementClient _managementClient, string queueName)
        {
            try
            {
                return _managementClient.GetQueueAsync(queueName).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                // it's OK... try and create it instead
            }

            try
            {
                return _managementClient.CreateQueueAsync(queueName).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (MessagingEntityAlreadyExistsException)
            {
                return _managementClient.GetQueueAsync(queueName).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                throw new ArgumentException($"Could not create Queue '{queueName}'", exception);
            }
        }
    }
}