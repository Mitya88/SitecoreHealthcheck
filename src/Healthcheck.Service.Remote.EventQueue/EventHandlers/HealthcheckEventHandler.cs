namespace Healthcheck.Service.Remote.EventQueue.EventHandlers
{
    using Healthcheck.Service.Core;
    using Healthcheck.Service.Core.Messages;
    using Healthcheck.Service.Core.Models.Event;
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using Newtonsoft.Json;
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Events;
    using Sitecore.Data.Items;
    using Sitecore.Diagnostics;
    using Sitecore.SecurityModel;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Constants = Core.Constants;

    public class HealthcheckEventHandler
    {
        public void HealthcheckStarted(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");

            var data = args as RemoteEventArgs<HealthcheckStartedRemoteEvent>;
            // Receving the data

            var messageContract = data.Event.ComponentData;

            if (!SharedConfig.InstanceName.Equals(messageContract.TargetInstance))
            {
                // TRigger only on the proper instance
                Sitecore.Diagnostics.Log.Info($"skipping remote event: {messageContract.TargetInstance}, current instance {Settings.InstanceName}", this);
                return;
            }
            HealthcheckResult result = null;

            if (data.Event.Type.Equals(Constants.TemplateNames.RemoteLogFileCheckTemplateName))
            {
                result = LogFileCheck.RunHealthcheck(messageContract.Parameters["FileNameFormat"], Path.Combine(Sitecore.Configuration.Settings.DataFolder, "logs"), DateTime.ParseExact(messageContract.Parameters["LastCheckTime"], "yyyyMMddTHHmmss", CultureInfo.InvariantCulture), int.Parse(messageContract.Parameters["NumberOfDaysToCheck"]), true);
            }
            else if (data.Event.Type.Equals(Constants.TemplateNames.RemoteCertificateCheckTemplateName))
            {
                result = CertificateCheck.RunHealthcheck(messageContract.Parameters["StoreName"], messageContract.Parameters["Location"], messageContract.Parameters["Value"], messageContract.Parameters["FindByType"], int.Parse(messageContract.Parameters["Warn Before"]));
            }
            else if (data.Event.Type.Equals(Constants.TemplateNames.RemoteXConnectApiCheckTemplateName))
            {
                result = XConnectApiCheck.RunHealthcheck(messageContract.Parameters["XConnectApiCertificateConnectionStringKey"], messageContract.Parameters["XConnectApiConnectionStringKey"], int.Parse(messageContract.Parameters["Warn Before"]));
            }
            else if (data.Event.Type.Equals(Constants.TemplateNames.RemoteApiHealthcheckTemplateName))
            {
                result = ApiCheck.RunHealthcheck(messageContract);
            }
            else if (data.Event.Type.Equals(Constants.TemplateNames.RemoteCustomHealthcheckTemplateName))
            {
                result = CustomCheck.RunHealthcheck(messageContract.Parameters["Type"], Sitecore.StringUtil.GetNameValues(messageContract.Parameters["Parameters"]));
            }
            else if (data.Event.Type.Equals(Constants.TemplateNames.RemoteDatabaseHealtcheckTemplateName))
            {
                result = DatabaseCheck.RunHealthcheck(messageContract.Parameters["ConnectionStringKey"]);
            }
            else if (data.Event.Type.Equals(Constants.TemplateNames.RemoteDiskSpaceCheckTemplateName))
            {
                result = DiskSpaceCheck.RunHealthcheck(messageContract.Parameters["DriveName"], int.Parse(messageContract.Parameters["ErrorPercentageThreshold"]), int.Parse(messageContract.Parameters["WarningPercentageThreshold"]));
            }
            else if (data.Event.Type.Equals(Constants.TemplateNames.RemoteLicenseHealthcheckTemplateName))
            {
                result = LicenseCheck.RunHealthcheck(int.Parse(messageContract.Parameters["WarnBefore"]), int.Parse(messageContract.Parameters["ErrorBefore"]));
            }
            else if (data.Event.Type.Equals(Constants.TemplateNames.RemoteWindowsServiceCheckTemplateName))
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

                // Sending result back
                var database = Sitecore.Configuration.Factory.GetDatabase("web");
                var eventQueue = database.RemoteEvents.EventQueue;

                var remoteEvent = new HealthcheckFinishedRemoteEvent("healthcheck:finished:remote", incomingMessage);
                eventQueue.QueueEvent<HealthcheckFinishedRemoteEvent>(remoteEvent, true, false);
            }
        }

        public void HealthcheckFinished(object sender, EventArgs args)
        {
            var data = args as RemoteEventArgs<HealthcheckFinishedRemoteEvent>;
            var messageContract = data.Event.ComponentData;
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
            Sitecore.Diagnostics.Log.Info(string.Format("HEALTHCHECKFINISHEDRAISED {0}", data.Event.InstanceName), this);
        }

        private string GetErrorMessagesJson(ErrorList errorList)
        {
            return JsonConvert.SerializeObject(errorList).Replace("\"SafeSerializationManager\":", "\"_SafeSerializationManager\":");
        }
    }
}