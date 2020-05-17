namespace Healthcheck.Service.Domain
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Models;
    using Sitecore.Data.Items;
    using System;
    using System.Configuration;
    using System.Data.SqlClient;

    /// <summary>
    /// Database healthcheck component
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.BaseComponent" />
    public class DatabaseHealthCheck : BaseComponent
    {
        /// <summary>
        /// Gets or sets the connection string key.
        /// </summary>
        /// <value>
        /// The connection string key.
        /// </value>
        public string ConnectionStringKey { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseHealthCheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public DatabaseHealthCheck(Item item) : base(item)
        {
            this.ConnectionStringKey = item["Connectionstring Key"];
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            this.LastCheckTime = DateTime.UtcNow;
            this.Status = HealthcheckStatus.Healthy;
            this.HealthyMessage = "Database Connection is OK";

            if (string.IsNullOrEmpty(ConnectionStringKey))
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Missing connectionstring key",
                    Exception = null
                });

                return;
            }

            if (ConfigurationManager.ConnectionStrings[ConnectionStringKey] == null)
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Cannot find the connectionstring in the config",
                    Exception = null
                });

                return;
            }

            try
            {
                var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnectionStringKey].ConnectionString);
                connection.Open();
                connection.Close();
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
    }
}