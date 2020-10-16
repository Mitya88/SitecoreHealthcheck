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
    using System.Collections.Specialized;
    using System.Text;

    /// <summary>
    /// Remote Custom healthcheck component
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.RemoteBaseComponent" />
    public class RemoteCustomHealthcheck : RemoteBaseComponent
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public NameValueCollection Parameters { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteCertificateCheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public RemoteCustomHealthcheck(Item item) : base(item)
        {
            this.Type = item["Type"];
            var parameters = item["Parameters"];
            this.Parameters = Sitecore.Web.WebUtil.ParseUrlParameters(parameters);
            this.InnerItem = item;
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            var dateTime = DateTime.UtcNow;
            this.SaveRemoteCheckStarted(dateTime);

            var parameters = string.Join("&", Array.ConvertAll(this.Parameters.AllKeys, key => string.Format("{0}={1}", key, this.Parameters[key])));

            var message = new OutGoingMessage
            {
                Parameters = new System.Collections.Generic.Dictionary<string, string>
                {
                    {"Type", this.Type },
                    {"Parameters", this.InnerItem["Parameters"] }
                },
                TargetInstance = this.TargetInstance,
                ComponentId = this.InnerItem.ID.Guid,
                EventRaised = dateTime
            };

            if (Settings.GetSetting("Healthcheck.Remote.Mode").Equals("eventqueue", StringComparison.OrdinalIgnoreCase))
            {
                var remoteEvent = new HealthcheckStartedRemoteEvent(Constants.TemplateNames.RemoteCustomHealthcheckTemplateName, "healthcheck:started:remote", message);
                var database = Sitecore.Configuration.Factory.GetDatabase("web");
                var eventQueue = database.RemoteEvents.EventQueue;
                eventQueue.QueueEvent<HealthcheckStartedRemoteEvent>(remoteEvent, true, false);
            }
            else
            {
                var busMessage = new Microsoft.Azure.ServiceBus.Message
                {
                    ContentType = "application/json",
                    Label = Constants.TemplateNames.RemoteCustomHealthcheckTemplateName,
                    Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message))
                };

                var messageSender = new MessageSender(SharedConfig.ConnectionStringOrKey, SharedConfig.TopicName);
                messageSender.SendAsync(busMessage).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
    }
}