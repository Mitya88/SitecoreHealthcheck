namespace Healthcheck.Service.Domain.Remote
{
    using Healthcheck.Service.Core;
    using Healthcheck.Service.Core.Messages;
    using Healthcheck.Service.Core.Senders;
    using Sitecore.Configuration;
    using Sitecore.Data.Items;
    using System;
    using System.IO;

    /// <summary>
    /// Remote Log file healthcheck
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.RemoteBaseComponent" />
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
        /// Initializes a new instance of the <see cref="RemoteLogFileHealthcheck"/> class.
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

            if(this.LastCheckTime == DateTime.MinValue)
            {
                this.LastCheckTime = DateTime.Now.AddDays(-this.NumberOfDaysToCheck);
            }
            var message = new OutGoingMessage
            {
                Parameters = new System.Collections.Generic.Dictionary<string, string>
                {
                    {"FileNameFormat", this.FileNameFormat },
                    {"ItemCreationDate", this.ItemCreationDate.ToString("yyyyMMddTHHmmss") },
                    {"NumberOfDaysToCheck", this.NumberOfDaysToCheck.ToString() },
                    {"LastCheckTime", this.LastCheckTime.ToString("yyyyMMddTHHmmss") }
                },
                TargetInstance = this.TargetInstance,
                ComponentId = this.InnerItem.ID.Guid,
                EventRaised = dateTime
            };

            if (Settings.GetSetting("Healthcheck.Remote.Mode").Equals("eventqueue", StringComparison.OrdinalIgnoreCase))
            {
                EventQueueSender.Send(Constants.TemplateNames.RemoteLogFileCheckTemplateName, message);
            }
            else
            {
                MessageBusSender.Send(Constants.TemplateNames.RemoteLogFileCheckTemplateName, message);
            }
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