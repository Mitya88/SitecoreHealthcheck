namespace Healthcheck.Service.Domain
{
    using Healthcheck.Service.Core;
    using Sitecore.Data.Items;
    using System;
    using System.IO;

    /// <summary>
    /// Log file healthcheck
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.BaseComponent" />
    public class LogFileHealthcheck : BaseComponent
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
        public LogFileHealthcheck(Item item) : base(item)
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
            var result = LogFileCheck.RunHealthcheck(this.FileNameFormat, this.GetLogFileDirectoryPath(), this.ItemCreationDate, this.NumberOfDaysToCheck);

            this.Status = result.Status;
            this.HealthyMessage = result.HealthyMessage;
            this.ErrorList = result.ErrorList;
            this.LastCheckTime = result.LastCheckTime;
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