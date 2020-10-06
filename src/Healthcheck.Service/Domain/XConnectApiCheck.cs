namespace Healthcheck.Service.Domain
{
    using Sitecore.Data.Items;

    /// <summary>
    /// Xconnect Api check component
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.BaseComponent" />
    public class XConnectApiCheck : BaseComponent
    {
        /// <summary>
        /// Gets or sets the x connect API connection string key.
        /// </summary>
        /// <value>
        /// The x connect API connection string key.
        /// </value>
        public string XConnectApiConnectionStringKey { get; set; }

        /// <summary>
        /// Gets or sets the x connect API certificate connection string key.
        /// </summary>
        /// <value>
        /// The x connect API certificate connection string key.
        /// </value>
        public string XConnectApiCertificateConnectionStringKey { get; set; }

        /// <summary>
        /// Gets or sets the warn before.
        /// </summary>
        /// <value>
        /// The warn before.
        /// </value>
        public int WarnBefore { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XConnectApiCheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public XConnectApiCheck(Item item) : base(item)
        {
            this.XConnectApiConnectionStringKey = item["XConnect Api ConnectionString Key"];
            this.XConnectApiCertificateConnectionStringKey = item["XConnect Certificate ConnectionString Key"];
            this.WarnBefore = Sitecore.MainUtil.GetInt(item["Warn Before"], 100);
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            var result = Healthcheck.Service.Core.XConnectApiCheck.RunHealthcheck(this.XConnectApiCertificateConnectionStringKey, this.XConnectApiConnectionStringKey, this.WarnBefore);

            this.Status = result.Status;
            this.HealthyMessage = result.HealthyMessage;
            this.ErrorList = result.ErrorList;
            this.LastCheckTime = result.LastCheckTime;
        }
    }
}