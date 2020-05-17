namespace Healthcheck.Service.Models
{
    using Newtonsoft.Json;

    /// <summary>
    /// WebJob class
    /// </summary>
    public class WebJobResponse
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}