namespace Healthcheck.Service.Domain
{
    using Sitecore;
    using Sitecore.Data.Items;
    using Sitecore.SecurityModel;
    using System;
    using System.Globalization;

    /// <summary>
    /// Abstract base class for remote components
    /// </summary>
    public abstract class RemoteBaseComponent:BaseComponent
    {
        /// <summary>
        /// Gets or sets the file name format.
        /// </summary>
        /// <value>
        /// The file name format.
        /// </value>
        public string TargetInstance { get; set; }

        /// <summary>
        /// Gets or sets the number of days to check.
        /// </summary>
        /// <value>
        /// The number of days to check.
        /// </value>
        public DateTime RemoteCheckStarted { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseComponent"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public RemoteBaseComponent(Item item):base(item)
        {
            this.TargetInstance = item["Target Instance"];
            this.RemoteCheckStarted = DateUtil.ParseDateTime(item["Remote Check Started"], DateTime.MinValue);
        }

        /// <summary>
        /// Saves the healthcheck result.
        /// </summary>
        /// <param name="numberOfDaysToKeepLogs">The number of days to keep logs.</param>
        public void SaveRemoteCheckStarted()
        {
            using (new SecurityDisabler())
            {
                using (new EditContext(this.InnerItem))
                {
                    this.InnerItem["Remote Check Started"] = DateUtil.FormatDateTime(this.RemoteCheckStarted, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture);                   
                }
            }
        }
    }
}