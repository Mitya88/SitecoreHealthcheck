namespace Healthcheck.Service.Checks.LogParsing
{
    using System.Collections.Generic;

    /// <summary>
    /// Logger
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// Get/Sets the Name of the Logger
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// Get/Sets the Level of the Logger
        /// </summary>
        /// <value></value>
        public LogLevel Level { get; set; }

        /// <summary>
        /// Gets the NoOfEntires of the Logger
        /// </summary>
        /// <value></value>
        public int NoOfEntires { get { return DataSource.Count; } }

        /// <summary>
        /// Gets the data source.
        /// </summary>
        /// <value>
        /// The data source.
        /// </value>
        public List<LogEntry> DataSource { get; }

        /// <summary>
        /// Initializes a new instance of the <b>Logger</b> class.
        /// </summary>
        /// <param name="logEntry"></param>
        public Logger(LogEntry logEntry)
        {
            Name = logEntry.Logger;
            Level = logEntry.Level;
            DataSource = new List<LogEntry>() { logEntry };
        }
    }
}