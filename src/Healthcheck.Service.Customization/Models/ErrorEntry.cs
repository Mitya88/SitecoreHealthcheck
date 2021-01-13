namespace Healthcheck.Service.Customization.Models
{
    using Newtonsoft.Json;
    using System;

    /// <summary>
    /// Error entity model
    /// </summary>
    public class ErrorEntry
    {
        /// <summary>
        /// Gets or sets the reason.
        /// </summary>
        /// <value>
        /// The reason.
        /// </value>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>
        /// The created.
        /// </value>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the serialized exception.
        /// </summary>
        /// <value>
        /// The serialized exception.
        /// </value>
        [JsonIgnore]
        public string SerializedException { get; set; }

        public ErrorLevel ErrorLevel { get; set; }

        /// <summary>Gets the default error entry.</summary>
        /// defaults: Created = DateTime.UtcNow
        ///           Exception = null
        /// <param name="reason">The reason.</param>
        /// <returns>Default ErrorEntry with a specific reason.</returns>
        public static ErrorEntry CreateErrorEntry(string reason, ErrorLevel errorLevel)
        {
            return new ErrorEntry()
            {
                Created = DateTime.UtcNow,
                Reason = reason,
                Exception = null,
                ErrorLevel = errorLevel
            };
        }

        public static ErrorEntry CreateErrorEntry(string reason, Exception exception)
        {
            return new ErrorEntry()
            {
                Created = DateTime.UtcNow,
                Reason = reason,
                Exception = exception,
                ErrorLevel = ErrorLevel.Error
            };
        }
    }
}