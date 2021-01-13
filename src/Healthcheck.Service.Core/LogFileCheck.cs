namespace Healthcheck.Service.Core
{
    using Healthcheck.Service.Core.LogParsing;
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Hosting;

    public class LogFileCheck
    {
        public static HealthcheckResult RunHealthcheck(string fileNameFormat, string directoryPath, DateTime itemCreationDate, int numberDaysToCheck, bool isRemote = false)
        {
            var checkResult = new HealthcheckResult
            {
                LastCheckTime = DateTime.UtcNow,
                Status = Customization.HealthcheckStatus.Healthy,
                HealthyMessage = "There is no new error since the last check",
                ErrorList = new ErrorList
                {
                    Entries = new List<ErrorEntry>()
                }
            };

            if (string.IsNullOrEmpty(fileNameFormat))
            {
                checkResult.Status = Customization.HealthcheckStatus.Warning;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Log File Check is not configured correctly",
                    Exception = null,
                    ErrorLevel = ErrorLevel.Warning
                });

                return checkResult;
            }

            try
            {
                var directory = GetLogFileDirectory(directoryPath);
                var files = directory.GetFiles(fileNameFormat);

                if (files == null || files.Count() == 0)
                {
                    checkResult.Status = HealthcheckStatus.Warning;
                    checkResult.ErrorList.Entries.Add(new ErrorEntry
                    {
                        Created = DateTime.UtcNow,
                        Reason = string.Format("No files can be found with the following pattern: {0}", fileNameFormat),
                        Exception = null,
                        ErrorLevel = ErrorLevel.Warning
                    });

                    return checkResult;
                }

                LogReaderSettings logReaderSettings = new LogReaderSettings(itemCreationDate, DateTime.MaxValue);

                if (numberDaysToCheck > 0)
                {
                    if (isRemote)
                    {
                        logReaderSettings.StartDateTime = itemCreationDate.ToLocalTime();
                    }
                    else
                    {
                        logReaderSettings.StartDateTime = DateTime.Now.AddDays(-numberDaysToCheck).Date;
                    }
                    logReaderSettings.FinishDateTime = DateTime.Now;
                }

                LogDataSource logDataSource = new LogDataSource(files, logReaderSettings);
                logDataSource.ParseFiles();

                var result = logDataSource.LogData;

                if (result.Errors != null && result.Errors.Count > 0)
                {
                    checkResult.Status = HealthcheckStatus.Error;
                    foreach (var error in result.Errors)
                    {
                        checkResult.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = error.Time,
                            Reason = error.Message?.Message,
                            Exception = null,
                            ErrorLevel = ErrorLevel.Error
                        });
                    }
                }
                else if (result.Warns != null && result.Warns.Count > 0)
                {
                    checkResult.Status = HealthcheckStatus.Warning;
                    foreach (var warn in result.Warns)
                    {
                        checkResult.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = warn.Time,
                            Reason = warn.Message?.Message,
                            Exception = null,
                            ErrorLevel = ErrorLevel.Warning
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                checkResult.Status = HealthcheckStatus.Error;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = exception.Message,
                    Exception = exception,
                    ErrorLevel = ErrorLevel.Error
                });
            }

            return checkResult;
        }

        /// <summary>
        /// Gets the log file directory.
        /// </summary>
        /// <returns>Directory info object.</returns>
        private static DirectoryInfo GetLogFileDirectory(string directory)
        {
            return new DirectoryInfo(GetFullPath(directory));
        }

        /// <summary>
        /// Gets the full path of a file..
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Mapped full path</returns>
        private static string GetFullPath(string fileName)
        {
            if (fileName.Contains(":\\"))
            {
                return fileName;
            }
            else
            {
                return HostingEnvironment.MapPath(fileName);
            }
        }
    }
}