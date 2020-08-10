namespace Healthcheck.Service.Customization.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Error list model
    /// </summary>
    public class ErrorList
    {
        /// <summary>
        /// Gets or sets the entries.
        /// </summary>
        /// <value>
        /// The entries.
        /// </value>
        public List<ErrorEntry> Entries { get; set; }
    }
}