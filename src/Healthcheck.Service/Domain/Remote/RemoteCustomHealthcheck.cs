namespace Healthcheck.Service.Domain.Remote
{
    using Healthcheck.Service.Core;
    using Healthcheck.Service.Core.Messages;
    using Healthcheck.Service.Core.Senders;
    using Sitecore.Configuration;
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Specialized;

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
                EventQueueSender.Send(Constants.TemplateNames.RemoteCustomHealthcheckTemplateName, message);
            }
            else
            {
                MessageBusSender.Send(Constants.TemplateNames.RemoteCustomHealthcheckTemplateName, message);
            }
        }
    }
}