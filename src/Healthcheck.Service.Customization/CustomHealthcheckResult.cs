namespace Healthcheck.Service.Customization
{
    using System;

    public class CustomHealthcheckResult
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
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the healthy message.
        /// </summary>
        /// <value>
        /// The healthy message.
        /// </value>
        public string HealthyMessage { get; set; }

        public Exception Exception { get; set; }

        public ErrorLevel ErrorLevel { get; set; }
    }
}