using Healthcheck.Service.Core.Messages;
using Healthcheck.Service.Customization;
using Healthcheck.Service.Customization.Models;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Healthcheck.Service.Remote.Messaging
{
    public class IncomingQueueMessageHandler
    {
        public static Task ReceiveMessage(Message message, CancellationToken token)
        {
            Sitecore.Diagnostics.Log.Info(Encoding.UTF8.GetString(message.Body), "MessageHandler");
            if (message.ContentType.Equals("application/json") && message.Label.Equals("Result"))
            {
                var messageContract = JsonConvert.DeserializeObject<HealthcheckResultMessage>(Encoding.UTF8.GetString(message.Body));

                using (new DatabaseSwitcher(Factory.GetDatabase("master")))
                {
                    var item = Sitecore.Context.Database.GetItem(new ID(messageContract.ComponentId));
                    using (new SecurityDisabler())
                    {
                        using (new EditContext(item))
                        {
                            item["Status"] = messageContract.Result.Status == HealthcheckStatus.UnKnown ? string.Empty : messageContract.Result.Status.ToString();
                            var errors = JsonConvert.DeserializeObject<ErrorList>(item["Error Messages"]);
                            if (errors.Entries == null)
                            {
                                errors.Entries = new List<ErrorEntry>();
                            }

                            errors.Entries.AddRange(messageContract.Result.ErrorList.Entries);
                            item["Error Messages"] = GetErrorMessagesJson(errors);
                            item["Healthy Message"] = messageContract.Result.HealthyMessage;
                            item["Last Check Time"] = DateUtil.FormatDateTime(messageContract.LastCheckTime, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture);
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        private static string GetErrorMessagesJson(ErrorList errorList)
        {
            return JsonConvert.SerializeObject(errorList).Replace("\"SafeSerializationManager\":", "\"_SafeSerializationManager\":");
        }

        public static Task LogMessageHandlerException(ExceptionReceivedEventArgs e)
        {
            Sitecore.Diagnostics.Log.Info(string.Format("Exception: \"{0}\" {1}", e.Exception.Message, e.ExceptionReceivedContext.EntityPath), "MessageHandler");
            return Task.CompletedTask;
        }
    }
}