namespace Healthcheck.Service.Domain.Remote
{
    using Healthcheck.Service.Core;
    using Healthcheck.Service.Core.Messages;
    using Microsoft.Azure.ServiceBus.Core;
    using Newtonsoft.Json;
    using Sitecore.Data.Items;
    using System;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Log file healthcheck
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.BaseComponent" />
    public class RemoteLogFileCheck : RemoteBaseComponent
    {
        /// <summary>
        /// Gets or sets the file name format.
        /// </summary>
        /// <value>
        /// The file name format.
        /// </value>
        public string FileNameFormat { get; set; }

        /// <summary>
        /// Gets or sets the number of days to check.
        /// </summary>
        /// <value>
        /// The number of days to check.
        /// </value>
        public int NumberOfDaysToCheck { get; set; }

        /// <summary>
        /// Gets or sets the item creation date.
        /// </summary>
        /// <value>
        /// The item creation date.
        /// </value>
        private DateTime ItemCreationDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogFileHealthcheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public RemoteLogFileCheck(Item item) : base(item)
        {
            this.FileNameFormat = item["File Name Format"];
            this.NumberOfDaysToCheck = Sitecore.MainUtil.GetInt(item["Number of Days to Check"], 0);
            this.ItemCreationDate = item.Created;
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
                    {"FileNameFormat", this.FileNameFormat },
                    {"ItemCreationDate", this.ItemCreationDate.ToString("yyyyMMddTHHmmss") },
                    {"NumberOfDaysToCheck", this.NumberOfDaysToCheck.ToString() }
                },
                TargetInstance = this.TargetInstance,
                ComponentId = this.InnerItem.ID.Guid,
                EventRaised = dateTime
            };

            var busMessage = new Microsoft.Azure.ServiceBus.Message
            {
                ContentType = "application/json",
                Label = Constants.TemplateNames.RemoteLogFileCheckTemplateName,
                Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message))
            };

            messageSender.SendAsync(busMessage).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Gets the log file directory path.
        /// </summary>
        /// <returns>Path to the directory.</returns>
        private string GetLogFileDirectoryPath()
        {
            return Path.Combine(Sitecore.Configuration.Settings.DataFolder, "logs");
        }
    }
}