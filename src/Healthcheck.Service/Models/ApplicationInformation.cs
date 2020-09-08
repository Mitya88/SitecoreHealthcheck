namespace Healthcheck.Service.Models
{
    /// <summary>
    /// Application information
    /// </summary>
    public class ApplicationInformation
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is administrator.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is administrator; otherwise, <c>false</c>.
        /// </value>
        public bool IsAdministrator { get; set; }

        public string MemoryUsage { get; set; }

        public string AvailableMemory { get; set; }

        public string CpuTime { get; set; }

        public string HardwareMemory { get; set; }
    }
}