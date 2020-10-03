namespace Healthcheck.Service.Remote
{
    using Healthcheck.Service.Core;
    using Healthcheck.Service.Remote.Messaging;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Management;
    using Sitecore.DependencyInjection;
    using Sitecore.Pipelines;
    using System;

    public class InitializeQueueClient
    {
        public virtual void Process(PipelineArgs args)
        {
            IQueueClient client = (IQueueClient)ServiceLocator.ServiceProvider.GetService(typeof(IQueueClient));

            var _managementClient = new ManagementClient(new ServiceBusConnectionStringBuilder(SharedConfig.ConnectionStringOrKey));
            EnsureQueueExists(_managementClient, SharedConfig.IncomingQueueName);

            client.RegisterMessageHandler(IncomingQueueMessageHandler.ReceiveMessage,
                new MessageHandlerOptions((e) => IncomingQueueMessageHandler.LogMessageHandlerException(e)) { AutoComplete = true, MaxConcurrentCalls = 1 });
        }

        private QueueDescription EnsureQueueExists(ManagementClient _managementClient, string queueName)
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