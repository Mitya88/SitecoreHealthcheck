namespace Healthcheck.Service.Domain
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using Healthcheck.Service.Models;
    using Sitecore.Data.Items;
    using Sitecore.Web;
    using System;
    using System.ServiceProcess;

    /// <summary>
    /// Keep Alive healthcheck component
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.BaseComponent" />
    public class KeepAliveCheck : BaseComponent
    {
        /// <summary>
        /// The keep alive URL
        /// </summary>
        private const string keepAliveUrl = "/sitecore/service/keepalive.aspx";

        /// <summary>
        /// Initializes a new instance of the <see cref="KeepAliveCheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public KeepAliveCheck(Item item) : base(item)
        {
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            this.LastCheckTime = DateTime.UtcNow;
            this.Status = HealthcheckStatus.Healthy;

            var fullUrl = WebUtil.GetFullUrl(keepAliveUrl);

            this.HealthyMessage = string.Format("The {0} url is working", fullUrl);

            try
            {
                WebUtil.ExecuteWebPage(fullUrl);
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