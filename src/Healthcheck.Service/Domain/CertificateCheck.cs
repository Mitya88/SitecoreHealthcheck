namespace Healthcheck.Service.Domain
{
    using Sitecore.Data.Items;

    /// <summary>
    /// Certificate check component
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.BaseComponent" />
    public class CertificateCheck : BaseComponent
    {
        /// <summary>
        /// Gets or sets the name of the store.
        /// </summary>
        /// <value>
        /// The name of the store.
        /// </value>
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the "find by type".
        /// </summary>
        /// <value>
        /// The find by type.
        /// </value>
        public string FindByType { get; set; }

        /// <summary>
        /// Gets or sets the warn before.
        /// </summary>
        /// <value>
        /// The warn before.
        /// </value>
        public int WarnBefore { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateCheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public CertificateCheck(Item item) : base(item)
        {
            this.StoreName = item["StoreName"];
            this.Location = item["Location"];
            this.Value = item["Value"];
            this.FindByType = item["FindByType"];
            int warnBefore = 100;

            this.WarnBefore = Sitecore.MainUtil.GetInt(item["Warn Before"], warnBefore);
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            var result = Healthcheck.Service.Core.CertificateCheck.RunHealthcheck(this.StoreName, this.Location, this.Value, this.FindByType, this.WarnBefore);

            this.Status = result.Status;
            this.HealthyMessage = result.HealthyMessage;
            this.ErrorList = result.ErrorList;
            this.LastCheckTime = result.LastCheckTime;
        }
    }
}