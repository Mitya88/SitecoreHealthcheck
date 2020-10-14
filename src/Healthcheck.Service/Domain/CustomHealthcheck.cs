namespace Healthcheck.Service.Domain
{
    using Sitecore.Data.Items;
    using System.Collections.Specialized;

    /// <summary>
    /// Custom healthcheck component
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.BaseComponent" />
    public class CustomHealthcheck : BaseComponent
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public NameValueCollection Parameters { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateCheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public CustomHealthcheck(Item item) : base(item)
        {
            this.Type = item["Type"];
            var parameters = item["Parameters"];
            this.Parameters = Sitecore.Web.WebUtil.ParseUrlParameters(parameters);
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            var result = Healthcheck.Service.Core.CustomCheck.RunHealthcheck(this.Type, this.Parameters);

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