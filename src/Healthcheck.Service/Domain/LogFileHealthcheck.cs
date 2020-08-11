namespace Healthcheck.Service.Domain
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using Healthcheck.Service.LogParsing;
    using Sitecore.Data.Items;
    using System;
    using System.IO;
    using System.Linq;
    using System.Web.Hosting;

    /// <summary>
    /// Log file healthcheck
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.BaseComponent" />
    public class LogFileHealthcheck : BaseComponent
    {
        /// <summary>
        /// Gets or sets the file name format.
        /// </summary>
        /// <value>
        /// The file name format.
        /// </value>
        public string FileNameFormat { get; set; }

        /// <summary>
        /// Gets or sets the number of days to check.
        /// </summary>
        /// <value>
        /// The number of days to check.
        /// </value>
        public int NumberOfDaysToCheck { get; set; }

        /// <summary>
        /// Gets or sets the item creation date.
        /// </summary>
        /// <value>
        /// The item creation date.
        /// </value>
        private DateTime ItemCreationDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogFileHealthcheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public LogFileHealthcheck(Item item) : base(item)
        {
            this.FileNameFormat = item["File Name Format"];
            this.NumberOfDaysToCheck = Sitecore.MainUtil.GetInt(item["Number of Days to Check"], 0);
            this.ItemCreationDate = item.Created;
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            this.LastCheckTime = DateTime.UtcNow;
            this.Status = HealthcheckStatus.Healthy;
            this.HealthyMessage = "Log files contain no errors";

            this.ErrorList.Entries.Clear();

            if (string.IsNullOrEmpty(FileNameFormat))
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Log File Check is not configured correctly",
                    Exception = null
                });

                return;
            }

            try
            {
                var directory = this.GetLogFileDirectory();
                var files = directory.GetFiles(FileNameFormat);

                if (files == null || files.Count() == 0)
                {
                    this.Status = HealthcheckStatus.Warning;
                    this.ErrorList.Entries.Add(new ErrorEntry
                    {
                        Created = DateTime.UtcNow,
                        Reason = string.Format("No files can be found with the following pattern: {0}", this.FileNameFormat),
                        Exception = null
                    });

                    return;
                }

                LogReaderSettings logReaderSettings = new LogReaderSettings(ItemCreationDate, DateTime.MaxValue);

                if (NumberOfDaysToCheck > 0)
                {
                    logReaderSettings.StartDateTime = DateTime.Now.AddDays(-NumberOfDaysToCheck).Date;
                    logReaderSettings.FinishDateTime = DateTime.Now;
                }

                LogDataSource logDataSource = new LogDataSource(files, logReaderSettings);
                logDataSource.ParseFiles();

                var result = logDataSource.LogData;

                if (result.Errors.Count > 0)
                {
                    this.Status = HealthcheckStatus.Error;
                    foreach (var error in result.Errors)
                    {
                        this.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = error.Time,
                            Reason = error.Message?.Message,
                            Exception = null
                        });
                    }
                }
                else if (result.Warns.Count > 0)
                {
                    this.Status = HealthcheckStatus.Warning;
                    foreach (var warn in result.Warns)
                    {
                        this.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = warn.Time,
                            Reason = warn.Message?.Message,
                            Exception = null
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                this.Status = HealthcheckStatus.Error;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = exception.Message,
                    Exception = exception
                });
            }
        }

        /// <summary>
        /// Gets the log file directory.
        /// </summary>
        /// <returns>Directory info object.</returns>
        private DirectoryInfo GetLogFileDirectory()
        {
            return new DirectoryInfo(this.GetFullPath(this.GetLogFileDirectoryPath()));
        }

        /// <summary>
        /// Gets the full path of a file..
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Mapped full path</returns>
        private string GetFullPath(string fileName)
        {
            return HostingEnvironment.MapPath(fileName);
        }

        /// <summary>
        /// Gets the log file directory path.
        /// </summary>
        /// <returns>Path to the directory.</returns>
        private string GetLogFileDirectoryPath()
        {
            return Path.Combine(Sitecore.Configuration.Settings.DataFolder, "logs");
        }
    }
}