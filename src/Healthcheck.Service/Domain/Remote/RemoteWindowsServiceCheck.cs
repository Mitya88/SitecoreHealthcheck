namespace Healthcheck.Service.Domain.Remote
{
    using Healthcheck.Service.Core;
    using Healthcheck.Service.Core.Messages;
    using Healthcheck.Service.Core.Senders;
    using Sitecore.Configuration;
    using Sitecore.Data.Items;
    using System;

    /// <summary>
    /// Remote Windows Service healthcheck component
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.RemoteBaseComponent" />
    public class RemoteWindowsServiceCheck : RemoteBaseComponent
    {
        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        public string ServiceName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteWindowsServiceCheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public RemoteWindowsServiceCheck(Item item) : base(item)
        {
            this.ServiceName = item["Service Name"];
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
                    {"ServiceName", this.ServiceName },
                    {"HealthyMessage",this.HealthyMessage }
                },
                TargetInstance = this.TargetInstance,
                ComponentId = this.InnerItem.ID.Guid,
                EventRaised = dateTime
            };

            if (Settings.GetSetting("Healthcheck.Remote.Mode").Equals("eventqueue", StringComparison.OrdinalIgnoreCase))
            {
                EventQueueSender.Send(Constants.TemplateNames.RemoteWindowsServiceCheckTemplateName, message);
            }
            else
            {
                MessageBusSender.Send(Constants.TemplateNames.RemoteWindowsServiceCheckTemplateName, message);
            }
        }
    }
}