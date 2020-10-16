namespace Healthcheck.Service.Domain.Remote
{
    using Healthcheck.Service.Core;
    using Healthcheck.Service.Core.Messages;
    using Healthcheck.Service.Remote.EventQueue.Models.Event;
    using Microsoft.Azure.ServiceBus.Core;
    using Newtonsoft.Json;
    using Sitecore.Configuration;
    using Sitecore.Data.Items;
    using System;
    using System.Text;

    /// <summary>
    /// Remote Licence check component
    /// </summary>
    public class RemoteLicenseCheck : RemoteBaseComponent
    {
        /// <summary>
        /// Gets or sets the warn before.
        /// </summary>
        /// <value>
        /// The warn before.
        /// </value>
        public int WarnBefore { get; set; }

        /// <summary>
        /// Gets or sets the error before.
        /// </summary>
        /// <value>
        /// The error before.
        /// </value>
        public int ErrorBefore { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteLicenseCheck"/> class.
        /// </summary>
        /// <param name="item"></param>
        public RemoteLicenseCheck(Item item) : base(item)
        {
            var warnBefore = 1;
            int.TryParse(item["WarnBefore"], out warnBefore);
            this.WarnBefore = warnBefore;
            var errorBefore = 1;
            int.TryParse(item["ErrorBefore"], out errorBefore);
            this.ErrorBefore = errorBefore;
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            var dateTime = DateTime.UtcNow;
            this.SaveRemoteCheckStarted(dateTime);

            var message = new OutGoingMessage
            {
                Parameters = new System.Collections.Generic.Dictionary<string, string>
                {
                    {"WarnBefore", this.WarnBefore.ToString() },
                    {"ErrorBefore", this.ErrorBefore.ToString() },
                },
                TargetInstance = this.TargetInstance,
                ComponentId = this.InnerItem.ID.Guid,
                EventRaised = dateTime
            };

            if (Settings.GetSetting("Healthcheck.Remote.Mode").Equals("eventqueue", StringComparison.OrdinalIgnoreCase))
            {
                var remoteEvent = new HealthcheckStartedRemoteEvent(Constants.TemplateNames.RemoteLicenseHealthcheckTemplateName, "healthcheck:started:remote", message);
                var database = Sitecore.Configuration.Factory.GetDatabase("web");
                var eventQueue = database.RemoteEvents.EventQueue;
                eventQueue.QueueEvent<HealthcheckStartedRemoteEvent>(remoteEvent, true, false);
            }
            else
            {
                var busMessage = new Microsoft.Azure.ServiceBus.Message
                {
                    ContentType = "application/json",
                    Label = Constants.TemplateNames.RemoteLicenseHealthcheckTemplateName,
                    Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message))
                };

                var messageSender = new MessageSender(SharedConfig.ConnectionStringOrKey, SharedConfig.TopicName);
                messageSender.SendAsync(busMessage).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
    }
}