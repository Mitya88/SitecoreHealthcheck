namespace Healthcheck.Service.Customization.Models
{
    using Healthcheck.Service.Customization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Sitecore;
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Component health model
    /// </summary>
    public class ComponentHealth
    {
        /// <summary>
        /// Gets or sets the error count.
        /// </summary>
        /// <value>
        /// The error count.
        /// </value>
        public int ErrorCount { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public HealthcheckStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the healthy message.
        /// </summary>
        /// <value>
        /// The healthy message.
        /// </value>
        public string HealthyMessage { get; set; }

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
        /// Initializes a new instance of the <see cref="ComponentHealth"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public ComponentHealth(Item item)
        {
            if (!string.IsNullOrEmpty(item["Status"]))
            {
                this.Status = (HealthcheckStatus)Enum.Parse(typeof(HealthcheckStatus), item["Status"]);
            }
            else
            {
                this.Status = HealthcheckStatus.UnKnown;
            }

            this.Name = string.IsNullOrEmpty(item["Name"]) ? item.Name : item["Name"];
            this.Id = item.ID.ToString();
            this.LastCheckTime = DateUtil.ParseDateTime(item["Last Check Time"], DateTime.MinValue);
            
            try
            {
                this.ErrorList = JsonConvert.DeserializeObject<ErrorList>(item["Error Messages"]);
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("HealthChecker - error messages can't be deserialized: " + item.Name, ex, this);
            }

            this.HealthyMessage = item["Healthy Message"];

            if (this.ErrorList == null)
            {
                this.ErrorList = new ErrorList();
            }

            if (this.ErrorList.Entries == null)
            {
                this.ErrorList.Entries = new List<ErrorEntry>();
            }

            this.ErrorList.Entries = this.ErrorList.Entries.OrderByDescending(t => t.Created).ToList();

            this.ErrorCount = this.ErrorList.Entries.Count;

            if(!string.IsNullOrEmpty(item["Remote Check Started"]))
            {
                var remoteCheck = DateUtil.ParseDateTime(item["Remote Check Started"], DateTime.MinValue);

                if (remoteCheck != this.LastCheckTime)
                {

                    if (remoteCheck.AddMinutes(1) >= DateTime.UtcNow)
                    {
                        this.Status = HealthcheckStatus.Waiting;
                    }
                    else
                    {
                        this.Status = HealthcheckStatus.Error;

                        this.ErrorList.Entries.Insert(0, new ErrorEntry
                        {
                            Created = DateTime.UtcNow,
                            Reason = "No Response Received from the Remote"
                        });

                        this.ErrorCount++;
                    }
                }
            }
        }
    }
}