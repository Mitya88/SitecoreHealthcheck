namespace Healthcheck.Service.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    /// <summary>
    /// CustomDimensions
    /// </summary>
    public class CustomDimensions
    {
        /// <summary>
        /// Gets or sets the name of the logger.
        /// </summary>
        /// <value>
        /// The name of the logger.
        /// </value>
        [JsonProperty("loggerName")]
        public string LoggerName { get; set; }
    }

    /// <summary>
    /// Trace
    /// </summary>
    public class Trace
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the severity level.
        /// </summary>
        /// <value>
        /// The severity level.
        /// </value>
        [JsonProperty("severityLevel")]
        public int SeverityLevel { get; set; }
    }

    /// <summary>
    /// Value
    /// </summary>
    public class Value
    {
        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the custom dimensions.
        /// </summary>
        /// <value>
        /// The custom dimensions.
        /// </value>
        [JsonProperty("customDimensions")]
        public CustomDimensions CustomDimensions { get; set; }

        /// <summary>
        /// Gets or sets the trace.
        /// </summary>
        /// <value>
        /// The trace.
        /// </value>
        [JsonProperty("trace")]
        public Trace Trace { get; set; }
    }

    /// <summary>
    /// Application Insight result model
    /// </summary>
    public class ApplicationInsightResult
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [JsonProperty("value")]
        public List<Value> Value { get; set; }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        /// <value>
        /// The status code.
        /// </value>
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets the warnings.
        /// </summary>
        /// <returns></returns>
        public List<Value> GetWarnings()
        {
            return Value.Where(p => p.Trace.SeverityLevel == (int)SeverityLevel.Warning).ToList();
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <returns></returns>
        public List<Value> GetErrors()
        {
            return Value.Where(p => p.Trace.SeverityLevel == (int)SeverityLevel.Error).ToList();
        }
    }

    /// <summary>
    /// Enum for severity levels
    /// </summary>
    public enum SeverityLevel
    {
        Warning = 2,
        Error = 3
    }
}