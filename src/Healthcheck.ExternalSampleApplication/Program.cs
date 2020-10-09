using Healthcheck.Service.Core;
using Healthcheck.Service.Core.Messages;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
//using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net.Configuration;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Healthcheck.ExternalSampleApplication
{
    class Program
    {
        public async Task Run(string connection, string topicName, string subName, string queueName)
        {
            subscriptionClient = new SubscriptionClient(connection, topicName, subName);

            _messageSender = new MessageSender(connection, queueName);
            this.InitializeReceiver(subscriptionClient, ConsoleColor.Green);

            await Task.WhenAny(
              Task.Run(() => Console.ReadKey()))
          ;

            await subscriptionClient.CloseAsync();
        }
        //static ManagementClient _managementClient;
         IMessageSender _messageSender;
        SubscriptionClient subscriptionClient;
        static string topicName = "";
        static string queueName = "";
        static string subName;

        public static int Main(string[]args)
        {
            try
            {
                var app = new Program();
                var connection = ConfigurationManager.ConnectionStrings["messaging"].ConnectionString;
                //_managementClient = new ManagementClient(new ServiceBusConnectionStringBuilder(connection));
                topicName = ConfigurationManager.AppSettings["TopicName"];
                queueName = ConfigurationManager.AppSettings["IncomingQueueName"];
                subName = ConfigurationManager.AppSettings["SubscritionName"];

                
                //var topic = GetTopic(topicName);
                //var queue = GetQueue(queueName);
                //var subscription = GetSubscription(topicName, subName);
                app.Run(connection, topicName, subName, queueName).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return 1;
            }
            return 0;
        }
        

        void InitializeReceiver(SubscriptionClient receiver, ConsoleColor color)
        {
            // register the RegisterMessageHandler callback
            receiver.RegisterMessageHandler(
                async (message, cancellationToken) =>
                {
                    //Console.WriteLine(Encoding.UTF8.GetString(message.Body), "MessageHandler");

                    if (message.ContentType.Equals("application/json"))
                    {
                        HealthcheckResult result = null;
                        var messageContract = JsonConvert.DeserializeObject<OutGoingMessage>(Encoding.UTF8.GetString(message.Body));

                        if (!subName.Equals(messageContract.TargetInstance, StringComparison.OrdinalIgnoreCase))
                        {
                            return;
                        }

                        if (message.Label.Equals(Constants.TemplateNames.RemoteLogFileCheckTemplateName))
                        {
                            result = LogFileCheck.RunHealthcheck(messageContract.Parameters["FileNameFormat"], ConfigurationManager.AppSettings["SitecoreLogFolder"], DateTime.ParseExact(messageContract.Parameters["ItemCreationDate"], "yyyyMMddTHHmmss", CultureInfo.InvariantCulture), int.Parse(messageContract.Parameters["NumberOfDaysToCheck"]));
                        }
                        else if (message.Label.Equals(Constants.TemplateNames.RemoteCertificateCheckTemplateName))
                        {
                            result = CertificateCheck.RunHealthcheck(messageContract.Parameters["StoreName"], messageContract.Parameters["Location"], messageContract.Parameters["Value"], messageContract.Parameters["FindByType"], int.Parse(messageContract.Parameters["Warn Before"]));
                        }
                        else if (message.Label.Equals(Constants.TemplateNames.RemoteXConnectApiCheckTemplateName))
                        {
                            result = XConnectApiCheck.RunHealthcheck(messageContract.Parameters["XConnectApiCertificateConnectionStringKey"], messageContract.Parameters["XConnectApiConnectionStringKey"], int.Parse(messageContract.Parameters["Warn Before"]));
                        }
                        else if (message.Label.Equals(Constants.TemplateNames.RemoteApiHealthcheckTemplateName))
                        {
                            result = ApiCheck.RunHealthcheck(messageContract);
                        }
                        else if (message.Label.Equals(Constants.TemplateNames.RemoteCustomHealthcheckTemplateName))
                        {
                            result = CustomCheck.RunHealthcheck(messageContract.Parameters["Type"], Sitecore.StringUtil.GetNameValues(messageContract.Parameters["Parameters"]));
                        }
                        else if (message.Label.Equals(Constants.TemplateNames.RemoteDatabaseHealtcheckTemplateName))
                        {
                            result = DatabaseCheck.RunHealthcheck(messageContract.Parameters["ConnectionStringKey"]);
                        }
                        else if (message.Label.Equals(Constants.TemplateNames.RemoteDiskSpaceCheckTemplateName))
                        {
                            result = DiskSpaceCheck.RunHealthcheck(messageContract.Parameters["DriveName"], int.Parse(messageContract.Parameters["ErrorPercentageThreshold"]), int.Parse(messageContract.Parameters["WarningPercentageThreshold"]));
                        }
                        else if (message.Label.Equals(Constants.TemplateNames.RemoteLicenseHealthcheckTemplateName))
                        {
                            result = LicenseCheck.RunHealthcheck(int.Parse(messageContract.Parameters["WarnBefore"]), int.Parse(messageContract.Parameters["ErrorBefore"]));
                        }
                        else if (message.Label.Equals(Constants.TemplateNames.RemoteWindowsServiceCheckTemplateName))
                        {
                            result = WindowsServiceCheck.RunHealthcheck(messageContract.Parameters["ServiceName"], messageContract.Parameters["HealthyMessage"]);
                        }

                        if (result != null)
                        {
                            

                            var incomingMessage = new HealthcheckResultMessage
                            {
                                Result = result,
                                ComponentId = messageContract.ComponentId,
                                LastCheckTime = messageContract.EventRaised
                            };
                            await _messageSender.SendAsync(new Message
                            {
                                Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(incomingMessage)),
                                ContentType = "application/json",
                                Label = "Result"
                            }).ConfigureAwait(false);
                        }
                    }
                },
                new MessageHandlerOptions((e) => LogMessageHandlerException(e)) { AutoComplete = true, MaxConcurrentCalls = 1 });
        }

        public static Task LogMessageHandlerException(ExceptionReceivedEventArgs e)
        {
            Console.WriteLine(e.Exception);
            return Task.CompletedTask;
        }

        //static TopicDescription GetTopic(string topicName)
        //{
        //    try
        //    {
        //        return _managementClient.GetTopicAsync(topicName).ConfigureAwait(false).GetAwaiter().GetResult();
        //    }
        //    catch (MessagingEntityNotFoundException)
        //    {
        //        // it's OK... try and create it instead
        //    }

        //    try
        //    {
        //        return _managementClient.CreateTopicAsync(topicName).ConfigureAwait(false).GetAwaiter().GetResult();
        //    }
        //    catch (MessagingEntityAlreadyExistsException)
        //    {
        //        return _managementClient.GetTopicAsync(topicName).ConfigureAwait(false).GetAwaiter().GetResult();
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new ArgumentException($"Could not create topic '{topicName}'", exception);
        //    }
        //}


        //static SubscriptionDescription GetSubscription(string topicName, string subscription)
        //{
        //    try
        //    {
        //        return _managementClient.GetSubscriptionAsync(topicName, subscription).ConfigureAwait(false).GetAwaiter().GetResult();
        //    }
        //    catch (MessagingEntityNotFoundException)
        //    {
        //        // it's OK... try and create it instead
        //    }

        //    try
        //    {
        //        return _managementClient.CreateSubscriptionAsync(topicName, subscription).ConfigureAwait(false).GetAwaiter().GetResult();
        //    }
        //    catch (MessagingEntityAlreadyExistsException)
        //    {
        //        return _managementClient.GetSubscriptionAsync(topicName, subscription).ConfigureAwait(false).GetAwaiter().GetResult();
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new ArgumentException($"Could not create topic '{topicName}'", exception);
        //    }
        //}       


        //static QueueDescription GetQueue(string queueName)
        //{
        //    try
        //    {
        //        return _managementClient.GetQueueAsync(queueName).ConfigureAwait(false).GetAwaiter().GetResult();
        //    }
        //    catch (MessagingEntityNotFoundException)
        //    {
        //        // it's OK... try and create it instead
        //    }

        //    try
        //    {
        //        return _managementClient.CreateQueueAsync(queueName).ConfigureAwait(false).GetAwaiter().GetResult();
        //    }
        //    catch (MessagingEntityAlreadyExistsException)
        //    {
        //        return _managementClient.GetQueueAsync(queueName).ConfigureAwait(false).GetAwaiter().GetResult();
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new ArgumentException($"Could not create topic '{topicName}'", exception);
        //    }
        //}
    }
}
