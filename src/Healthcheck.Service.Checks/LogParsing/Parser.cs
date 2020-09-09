namespace Healthcheck.Service.Checks.LogParsing
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The log parser
    /// </summary>
    public class Parser
    {
        private LogEntry current;
        private readonly LogData logData;

        /// <summary>
        /// The last used pattern
        /// </summary>
        private LogPattern lastUsedPattern;

        private DateTime logFileDate;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public Parser(LogData data, DateTime logFileDate)
        {
            logData = data;
            this.logFileDate = logFileDate;
        }

        /// <summary>
        /// Parses the line.
        /// </summary>
        /// <param name="line">The line.</param>
        public void ParseLine(string line)
        {
            var entry = ParseEntry(line);
            if (entry != null)
            {
                entry.Time = new DateTime(logFileDate.Year, logFileDate.Month, logFileDate.Day, entry.Time.Hour, entry.Time.Minute, entry.Time.Second);

                current = entry;
                logData.Add(entry);
            }
            else
            {
                if (current == null)
                {
                    current = new LogEntry { Message = new LogMessage(line) };
                }
                else
                {
                    current.Message.Message += Environment.NewLine + line;
                }
            }
        }

        /// <summary>
        /// The log patterns
        /// </summary>
        private readonly List<LogPattern> logPatterns = new List<LogPattern> {
          new LogPattern(@"^(?<process>\S+)\s+(?<date>\d{2}:\d{2}:\d{2})\s+(?<level>\w+)\s+(?<message>.*)$","HH:mm:ss"),
          new LogPattern(@"^(?<process>\S+)\s+(?<thread>\S+)\s+(?<date>\d{2}:\d{2}:\d{2})\s+(?<level>\w+)\s+(?<message>.*)$","HH:mm:ss"),
        };

        /// <summary>
        /// Gets the log patterns.
        /// </summary>
        /// <value>
        /// The log patterns.
        /// </value>
        private IEnumerable<LogPattern> LogPatterns
        {
            get
            {
                if (lastUsedPattern != null)
                    yield return lastUsedPattern;
                var oldLastUsedPattern = lastUsedPattern;

                foreach (var logPattern in logPatterns)
                {
                    //skip last used pattern (returned first)
                    if (oldLastUsedPattern == logPattern) continue;

                    lastUsedPattern = logPattern;

                    yield return lastUsedPattern;
                }
            }
        }

        /// <summary>
        /// Parses the entry.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        private LogEntry ParseEntry(string line)
        {
            foreach (var logPattern in LogPatterns)
            {
                var match = Regex.Match(line, logPattern.Pattern, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
                if (match.Success)
                {
                    string thread = null;
                    if (match.Groups["thread"].Success)
                    {
                        thread = match.Groups["thread"].Value;
                    }
                    string process = null;
                    if (match.Groups["process"].Success)
                    {
                        process = match.Groups["process"].Value;
                    }
                    var entry = new LogEntry
                    {
                        Time = DateTime.ParseExact(match.Groups["date"].Value, logPattern.DateTimeFormat, CultureInfo.InvariantCulture),
                        Level = (LogLevel)Enum.Parse(typeof(LogLevel), match.Groups["level"].Value),
                        Thread = thread,
                        Process = process,
                        Logger = match.Groups["logger"].Value,
                        Message = new LogMessage(match.Groups["message"].Value)
                    };
                    return entry;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Log pattern
    /// </summary>
    public class LogPattern
    {
        /// <summary>
        /// Gets the pattern.
        /// </summary>
        /// <value>
        /// The pattern.
        /// </value>
        public string Pattern { get; }

        /// <summary>
        /// Gets the date time format.
        /// </summary>
        /// <value>
        /// The date time format.
        /// </value>
        public string DateTimeFormat { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogPattern"/> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <param name="dateTimeFormat">The date time format.</param>
        public LogPattern(string pattern, string dateTimeFormat)
        {
            Pattern = pattern;
            DateTimeFormat = dateTimeFormat;
        }
    }
}