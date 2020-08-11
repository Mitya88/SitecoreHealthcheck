namespace Healthcheck.Service.LogParsing
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Log data
    /// </summary>
    public class LogData
    {
        /// <summary>
        /// The loggers
        /// </summary>
        private readonly Dictionary<string, Logger> loggers = new Dictionary<string, Logger>();

        /// <summary>
        /// Gets the logger data source.
        /// </summary>
        /// <value>
        /// The logger data source.
        /// </value>
        public List<Logger> LoggerDataSource { get; }

        /// <summary>
        /// Gets the log entry data source.
        /// </summary>
        /// <value>
        /// The log entry data source.
        /// </value>
        public List<LogEntry> LogEntryDataSource { get; }

        /// <summary>
        /// Gets the warnings.
        /// </summary>
        /// <value>
        /// The warnings.
        /// </value>
        public List<LogEntry> Warns
        {
            get
            {
                return LoggerDataSource.Where(p => p.Level == LogLevel.WARN).FirstOrDefault()?.DataSource;
            }
        }

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <value>
        /// The errors.
        /// </value>
        public List<LogEntry> Errors
        {
            get
            {
                return LoggerDataSource.Where(p => p.Level == LogLevel.ERROR).FirstOrDefault()?.DataSource;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogData"/> class.
        /// </summary>
        public LogData()
        {
            //_invoke = invoke;
            LoggerDataSource = new List<Logger>();
            LogEntryDataSource = new List<LogEntry>();
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            loggers.Clear();
        }

        public void Add(LogEntry entry)
        {
            LogEntryDataSource.Add(entry);
            var key = $"{entry.Level}|{entry.Logger}";
            if (!loggers.ContainsKey(key))
            {
                var logger = new Logger(entry);
                loggers.Add(key, logger);
                LoggerDataSource.Add(logger);
            }
            else
            {
                loggers[key].DataSource.Add(entry);
            }
        }
    }
}