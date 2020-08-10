namespace Healthcheck.Service.Domain
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using Healthcheck.Service.Utilities;
    using Newtonsoft.Json;
    using Sitecore;
    using Sitecore.Data.Items;
    using Sitecore.SecurityModel;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Abstract base class for components
    /// </summary>
    public abstract class BaseComponent
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public HealthcheckStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the error messages.
        /// </summary>
        /// <value>
        /// The error messages.
        /// </value>
        public string ErrorMessages { get; set; }

        /// <summary>
        /// Gets or sets the last check time.
        /// </summary>
        /// <value>
        /// The last check time.
        /// </value>
        public DateTime LastCheckTime { get; set; }

        /// <summary>
        /// Gets or sets the error list.
        /// </summary>
        /// <value>
        /// The error list.
        /// </value>
        public ErrorList ErrorList { get; set; }

        /// <summary>
        /// Gets or sets the healthy message.
        /// </summary>
        /// <value>
        /// The healthy message.
        /// </value>
        public string HealthyMessage { get; set; }

        /// <summary>
        /// Gets or sets the inner item.
        /// </summary>
        /// <value>
        /// The inner item.
        /// </value>
        public Item InnerItem { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseComponent"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public BaseComponent(Item item)
        {
            if (!string.IsNullOrEmpty(item["Status"]))
            {
                this.Status = (HealthcheckStatus)Enum.Parse(typeof(HealthcheckStatus), item["Status"]);
            }
            else
            {
                this.Status = HealthcheckStatus.UnKnown;
            }

            this.ErrorMessages = item["Error Messages"];
            this.HealthyMessage = item["Healthy Message"];
            this.LastCheckTime = DateUtil.ParseDateTime(item["Last Check Time"], DateTime.MinValue);
            try
            {
                this.ErrorList = JsonConvert.DeserializeObject<ErrorList>(this.ErrorMessages);
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("HealthChecker - error messages can't be deserialized: " + item.Name, ex, this);
            }

            this.InnerItem = item;

            if (this.ErrorList == null)
            {
                this.ErrorList = new ErrorList();
            }

            if (this.ErrorList.Entries == null)
            {
                this.ErrorList.Entries = new List<ErrorEntry>();
            }
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public abstract void RunHealthcheck();

        /// <summary>
        /// Saves the healthcheck result.
        /// </summary>
        /// <param name="numberOfDaysToKeepLogs">The number of days to keep logs.</param>
        public void SaveHealthcheckResult(int numberOfDaysToKeepLogs)
        {
            this.ErrorList.Entries = this.ErrorList.Entries.Where(t => t.Created >= DateTime.UtcNow.AddDays(-numberOfDaysToKeepLogs)).ToList();

            using (new SecurityDisabler())
            {
                using (new EditContext(this.InnerItem))
                {
                    this.InnerItem["Status"] = this.Status == HealthcheckStatus.UnKnown ? string.Empty : this.Status.ToString();
                    this.InnerItem["Error Messages"] = JsonUtil.GetErrorMessagesJson(this.ErrorList);
                    this.InnerItem["Healthy Message"] = this.HealthyMessage;
                    this.InnerItem["Last Check Time"] = DateUtil.FormatDateTime(this.LastCheckTime, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture);
                }
            }
        }
    }
}