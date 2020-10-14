namespace Healthcheck.Service.Domain
{
    using Sitecore.Data.Items;

    /// <summary>
    /// Licence check component
    /// </summary>
    public class LicenseCheck : BaseComponent
    {
        /// <summary>
        /// Gets or sets the warn before.
        /// </summary>
        /// <value>
        /// The warn before.
        /// </value>
        public int WarnBefore { get; set; }

        /// <summary>
        /// Gets or sets the error before.
        /// </summary>
        /// <value>
        /// The error before.
        /// </value>
        public int ErrorBefore { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseCheck"/> class.
        /// </summary>
        /// <param name="item"></param>
        public LicenseCheck(Item item) : base(item)
        {
            var warnBefore = 1;
            int.TryParse(item["WarnBefore"], out warnBefore);
            this.WarnBefore = warnBefore;
            var errorBefore = 1;
            int.TryParse(item["ErrorBefore"], out errorBefore);
            this.ErrorBefore = errorBefore;
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            var result = Healthcheck.Service.Core.LicenseCheck.RunHealthcheck(this.WarnBefore, this.ErrorBefore);

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