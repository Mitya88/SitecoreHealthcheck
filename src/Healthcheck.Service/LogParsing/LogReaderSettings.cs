namespace Healthcheck.Service.LogParsing
{
    using System;

    /// <summary>
    /// Log reader settings
    /// </summary>
    public class LogReaderSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogReaderSettings"/> class.
        /// </summary>
        public LogReaderSettings()
          : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogReaderSettings"/> class.
        /// </summary>
        /// <param name="filter">The filter.</param>
        public LogReaderSettings(string filter)
          : this(DateTime.MinValue, DateTime.MaxValue, filter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogReaderSettings"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="finish">The finish.</param>
        public LogReaderSettings(DateTime start, DateTime finish)
          : this(start, finish, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogReaderSettings"/> class.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="finish">The finish.</param>
        /// <param name="filter">The filter.</param>
        public LogReaderSettings(DateTime start, DateTime finish, string filter)
        {
            if (!string.IsNullOrEmpty(filter))
            {
                this.Filter = filter.Trim(' ', '\n');
            }

            this.StartDateTime = start;
            this.FinishDateTime = finish;
        }

        /// <summary>
        /// Gets or sets the start date time.
        /// </summary>
        /// <value>
        /// The start date time.
        /// </value>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the finish date time.
        /// </summary>
        /// <value>
        /// The finish date time.
        /// </value>
        public DateTime FinishDateTime { get; set; }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public string Filter { get; set; }
    }
}