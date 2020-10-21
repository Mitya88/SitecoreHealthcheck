namespace Healthcheck.Service.Domain
{
    using Sitecore.Data.Items;

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
            var result = Healthcheck.Service.Core.DatabaseCheck.RunHealthcheck(this.ConnectionStringKey);

            this.Status = result.Status;
            this.HealthyMessage = result.HealthyMessage;
            if (this.ErrorList == null || this.ErrorList.Entries == null)
            {
                this.ErrorList = result.ErrorList;
            }
            else if (this.ErrorList != null && this.ErrorList.Entries != null)
            {
                this.ErrorList.Entries.AddRange(result.ErrorList.Entries);
            }
            this.LastCheckTime = result.LastCheckTime;
        }
    }
}