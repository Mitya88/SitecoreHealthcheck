using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Management;
using System;
using System.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Healthcheck.ExternalSampleApplication
{
    class Program
    {
        static ManagementClient _managementClient;
        static string topicName = "";
        static string queueName = "";
        static string subName;
        static void Main(string[] args)
        {
            var connection = ConfigurationManager.ConnectionStrings["messaging"].ConnectionString;
            _managementClient = new ManagementClient(new ServiceBusConnectionStringBuilder(connection));
            topicName = ConfigurationManager.AppSettings["TopicName"];
            queueName = ConfigurationManager.AppSettings["IncomingQueueName"];
            subName = ConfigurationManager.AppSettings["SubscritionName"];
            var topic = GetTopic(topicName);
            var queue = GetQueue(queueName);
            var subscription = GetSubscription(topicName, subName);
            ITopicClient client = new TopicClient(connection, topicName);
            


            var messageSender = new MessageSender(connection, topicName);
            messageSender.SendAsync(new Message
            {
                Body = Encoding.UTF8.GetBytes("hello")
            }).ConfigureAwait(false).GetAwaiter().GetResult();

           
                }

        static TopicDescription GetTopic(string topicName)
        {
            try
            {
                return _managementClient.GetTopicAsync(topicName).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                // it's OK... try and create it instead
            }

            try
            {
                return _managementClient.CreateTopicAsync(topicName).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (MessagingEntityAlreadyExistsException)
            {
                return _managementClient.GetTopicAsync(topicName).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                throw new ArgumentException($"Could not create topic '{topicName}'", exception);
            }
        }


        static SubscriptionDescription GetSubscription(string topicName, string subscription)
        {
            try
            {
                return _managementClient.GetSubscriptionAsync(topicName, subscription).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                // it's OK... try and create it instead
            }

            try
            {
                return _managementClient.CreateSubscriptionAsync(topicName, subscription).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (MessagingEntityAlreadyExistsException)
            {
                return _managementClient.GetSubscriptionAsync(topicName, subscription).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                throw new ArgumentException($"Could not create topic '{topicName}'", exception);
            }
        }

        private static async Task ReceiveMessage(Message message, CancellationToken token)
        {
            Console.WriteLine(Encoding.UTF8.GetString(message.Body));
        }

        private static Task LogMessageHandlerException(ExceptionReceivedEventArgs e)
        {
            Console.WriteLine("Exception: \"{0}\" {1}", e.Exception.Message, e.ExceptionReceivedContext.EntityPath);
            return Task.CompletedTask;
        }


        static QueueDescription GetQueue(string queueName)
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
                throw new ArgumentException($"Could not create topic '{topicName}'", exception);
            }
        }
    }
}
