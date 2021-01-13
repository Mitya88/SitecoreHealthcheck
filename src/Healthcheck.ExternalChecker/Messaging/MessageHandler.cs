namespace Healthcheck.ExternalChecker
{
    using Healthcheck.Service.Core;
    using Healthcheck.Service.Core.Messages;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Azure.ServiceBus.Core;
    using Newtonsoft.Json;
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class MessageHandler
    {
        public static async Task ReceiveMessage(Message message, CancellationToken token)
        {
            if (message.ContentType.Equals("application/json"))
            {
                HealthcheckResult result = null;
                var messageContract = JsonConvert.DeserializeObject<OutGoingMessage>(Encoding.UTF8.GetString(message.Body));

                //if (!SharedConfig.SubscriptionName.Equals(messageContract.TargetInstance, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(messageContract.TargetInstance))
                //{
                //    // Trigger only on the proper instance
                //    //Sitecore.Diagnostics.Log.Info($"[Advanced.Healthcheck] - Skipping remote event: {messageContract.TargetInstance}, current instance/Subscription {SharedConfig.SubscriptionName}", message);
                //    return;
                //}

                if (message.Label.Equals(Constants.TemplateNames.RemoteLogFileCheckTemplateName))
                {
                    result = LogFileCheck.RunHealthcheck(messageContract.Parameters["FileNameFormat"], messageContract.Parameters["LogFolder"], DateTime.ParseExact(messageContract.Parameters["LastCheckTime"], "yyyyMMddTHHmmss", CultureInfo.InvariantCulture), int.Parse(messageContract.Parameters["NumberOfDaysToCheck"]), true);
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
                    //result = CustomCheck.RunHealthcheck(messageContract.Parameters["Type"], Sitecore.StringUtil.GetNameValues(messageContract.Parameters["Parameters"]));
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
                    var sender = new MessageSender(ConfigurationManager.ConnectionStrings["sb"].ConnectionString, ConfigurationManager.AppSettings["Healthcheck.IncomingQueueName"]);

                    var incomingMessage = new HealthcheckResultMessage
                    {
                        Result = result,
                        ComponentId = messageContract.ComponentId,
                        LastCheckTime = messageContract.EventRaised
                    };
                    await sender.SendAsync(new Message
                    {
                        Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(incomingMessage)),
                        ContentType = "application/json",
                        Label = "Result"
                    }).ConfigureAwait(false);
                }
            }
        }

        public static Task LogMessageHandlerException(ExceptionReceivedEventArgs e)
        {
            //Sitecore.Diagnostics.Log.Info(string.Format("Exception: \"{0}\" {1}", e.Exception.Message, e.ExceptionReceivedContext.EntityPath), "MessageHandler");
            return Task.CompletedTask;
        }
    }
}