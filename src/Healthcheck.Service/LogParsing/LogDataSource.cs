namespace Healthcheck.Service.LogParsing
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Web;

    /// <summary>
    /// Log data source
    /// </summary>
    public class LogDataSource
    {
        /// <summary>
        /// Gets the log data.
        /// </summary>
        /// <value>
        /// The log data.
        /// </value>
        public LogData LogData { get; }
        
        private FileInfo[] logFiles;
        private LogReaderSettings logReaderSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogDataSource"/> class.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="logReaderSettings">The log reader settings.</param>
        public LogDataSource(FileInfo[] files, LogReaderSettings logReaderSettings)
        {
            LogData = new LogData();
            this.logFiles = files;
            this.logReaderSettings = logReaderSettings;
        }

        /// <summary>
        /// Parses the files.
        /// </summary>
        public void ParseFiles()
        {
            var filteredFileList = FilterFiles(logFiles, logReaderSettings);

            foreach (var fileInfo in filteredFileList)
            {
                ParseFile(fileInfo.FullName);
            }
        }

        /// <summary>
        /// Filters the files.
        /// </summary>
        /// <param name="logFiles">The log files.</param>
        /// <param name="logReaderSettings">The log reader settings.</param>
        /// <returns></returns>
        private FileInfo[] FilterFiles(FileInfo[] logFiles, LogReaderSettings logReaderSettings)
        {
            return logFiles.Where(p => GetDateFromFileName(p.Name) >= logReaderSettings.StartDateTime && GetDateFromFileName(p.Name) <= logReaderSettings.FinishDateTime).ToArray();
        }

        /// <summary>
        /// Parses the file.
        /// </summary>
        /// <param name="path">The path.</param>
        public void ParseFile(string path)
        {
            var fileDate = GetDateFromFileName(path);
            
            using (var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var parser = new Parser(LogData, fileDate);
                using (var reader = new StreamReader(file))
                {
                    string line;
                    var noOfLines = 0;
                    while ((line = reader.ReadLine()) != null)
                    {
                        parser.ParseLine(line);
                        noOfLines++;
                    }
                    
                    Debug.WriteLine($"Added {noOfLines} lines");
                }
            }
        }

        /// <summary>
        /// Gets the name of the date from file.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        private DateTime GetDateFromFileName(string line)
        {
            for (int index = 0; index < line.Length; ++index)
            {
                if (line[index] == '.' && index + 9 < line.Length && (char.IsDigit(line[index + 1]) && char.IsDigit(line[index + 2])) && (char.IsDigit(line[index + 3]) && char.IsDigit(line[index + 4]) && (char.IsDigit(line[index + 5]) && char.IsDigit(line[index + 6]))) && (char.IsDigit(line[index + 7]) && char.IsDigit(line[index + 8])))
                {
                    int year = ((int)line[index + 1] - 48) * 1000 + ((int)line[index + 2] - 48) * 100 + ((int)line[index + 3] - 48) * 10 + (int)line[index + 4] - 48;
                    int month = ((int)line[index + 5] - 48) * 10 + (int)line[index + 6] - 48;
                    int day = ((int)line[index + 7] - 48) * 10 + (int)line[index + 8] - 48;
                    if (line[index + 9] != '.' || !char.IsDigit(line[index + 10]) || (!char.IsDigit(line[index + 11]) || !char.IsDigit(line[index + 12])) || (!char.IsDigit(line[index + 13]) || !char.IsDigit(line[index + 14]) || !char.IsDigit(line[index + 15])))
                        return new DateTime(year, month, day, 0, 0, 0);
                    int hour = ((int)line[index + 10] - 48) * 10 + ((int)line[index + 11] - 48);
                    int minute = ((int)line[index + 12] - 48) * 10 + ((int)line[index + 13] - 48);
                    int second = ((int)line[index + 14] - 48) * 10 + ((int)line[index + 15] - 48);
                    return new DateTime(year, month, day, hour, minute, second);
                }
            }

            return DateTime.MinValue;
        }
    }
}