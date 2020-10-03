namespace Healthcheck.Service.Domain.Remote
{
    using Healthcheck.Service.Core;
    using Healthcheck.Service.Core.Messages;
    using Microsoft.Azure.ServiceBus.Core;
    using Newtonsoft.Json;
    using Sitecore.Data.Items;
    using System;
    using System.Text;

    /// <summary>
    /// Remote Database healthcheck component
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.RemoteBaseComponent" />
    public class RemoteDatabaseHealthCheck : RemoteBaseComponent
    {
        /// <summary>
        /// Gets or sets the connection string key.
        /// </summary>
        /// <value>
        /// The connection string key.
        /// </value>
        public string ConnectionStringKey { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteDatabaseHealthCheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public RemoteDatabaseHealthCheck(Item item) : base(item)
        {
            this.ConnectionStringKey = item["Connectionstring Key"];
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            var dateTime = DateTime.UtcNow;
            this.SaveRemoteCheckStarted(dateTime);

            var messageSender = new MessageSender(SharedConfig.ConnectionStringOrKey, SharedConfig.TopicName);

            var message = new OutGoingMessage
            {
                Parameters = new System.Collections.Generic.Dictionary<string, string>
                {
                    {"ConnectionStringKey", this.ConnectionStringKey }
                },
                TargetInstance = this.TargetInstance,
                ComponentId = this.InnerItem.ID.Guid,
                EventRaised = dateTime
            };

            var busMessage = new Microsoft.Azure.ServiceBus.Message
            {
                ContentType = "application/json",
                Label = Constants.TemplateNames.RemoteDatabaseHealtcheckTemplateName,
                Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message))
            };

            messageSender.SendAsync(busMessage).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}