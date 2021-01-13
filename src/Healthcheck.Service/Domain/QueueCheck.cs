namespace Healthcheck.Service.Domain
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using Sitecore;
    using Sitecore.Data.Items;
    using System;
    using System.Configuration;
    using System.Data.SqlClient;

    /// <summary>
    /// Queue healthcheck component
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.BaseComponent" />
    public class QueueCheck : BaseComponent
    {
        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public string Database { get; set; }

        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        /// <value>
        /// The table.
        /// </value>
        public string Table { get; set; }

        /// <summary>
        /// Gets or sets the warn count.
        /// </summary>
        /// <value>
        /// The warn count.
        /// </value>
        public int WarnCount { get; set; }

        /// <summary>
        /// Gets or sets the error count.
        /// </summary>
        /// <value>
        /// The error count.
        /// </value>
        public int ErrorCount { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseHealthCheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public QueueCheck(Item item) : base(item)
        {
            this.Database = item["Database"];
            this.Table = item["Table"];

            this.WarnCount = MainUtil.GetInt(item["Warn Count"], 100);
            this.ErrorCount = MainUtil.GetInt(item["Error Count"], 100);
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            this.LastCheckTime = DateTime.UtcNow;
            this.Status = HealthcheckStatus.Healthy;
            this.HealthyMessage = "Database Connection is OK";

            if (string.IsNullOrEmpty(this.Database) || string.IsNullOrEmpty(this.Table))
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Missing database or table configuration",
                    Exception = null,
                    ErrorLevel = ErrorLevel.Warning
                });

                return;
            }

            try
            {
                string selectCount = string.Format("select count(*) from {0}", this.Table);

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[this.Database].ConnectionString))
                {
                    SqlCommand topiccmd = new SqlCommand(selectCount, connection);
                    connection.Open();
                    int numrows = (int)topiccmd.ExecuteScalar();
                    if (numrows >= this.WarnCount && numrows < this.ErrorCount)
                    {
                        this.Status = HealthcheckStatus.Warning;
                        this.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = DateTime.UtcNow,
                            Reason = string.Format("{0} table in {1} database has {2} record", this.Table, this.Database, numrows),
                            Exception = null,
                            ErrorLevel = ErrorLevel.Warning
                        });
                    }
                    else if (numrows >= this.ErrorCount)
                    {
                        this.Status = HealthcheckStatus.Error;
                        this.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = DateTime.UtcNow,
                            Reason = string.Format("{0} table in {1} database has {2} record", this.Table, this.Database, numrows),
                            Exception = null,
                            ErrorLevel = ErrorLevel.Error
                        });
                    }
                    else
                    {
                        this.HealthyMessage = string.Format("{0} table in {1} database has {2} record", this.Table, this.Database, numrows);
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
                    Exception = exception,
                    ErrorLevel = ErrorLevel.Error
                });
            }
        }
    }
}