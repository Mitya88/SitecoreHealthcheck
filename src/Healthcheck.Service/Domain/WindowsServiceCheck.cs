namespace Healthcheck.Service.Domain
{
    using Sitecore.Data.Items;

    /// <summary>
    /// Windows Service healthcheck component
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.BaseComponent" />
    public class WindowsServiceCheck : BaseComponent
    {
        /// <summary>
        /// Gets or sets the name of the service.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        public string ServiceName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseHealthCheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public WindowsServiceCheck(Item item) : base(item)
        {
            this.ServiceName = item["Service Name"];
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            var result = Healthcheck.Service.Core.WindowsServiceCheck.RunHealthcheck(this.ServiceName, this.HealthyMessage);

            this.Status = result.Status;
            this.HealthyMessage = result.HealthyMessage;
            this.ErrorList = result.ErrorList;
            this.LastCheckTime = result.LastCheckTime;
        }
    }
}