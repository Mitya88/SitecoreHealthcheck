namespace Healthcheck.Service.Domain
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using Healthcheck.Service.Models;
    using Sitecore.Data.Items;
    using System;
    using System.ServiceProcess;

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
            this.LastCheckTime = DateTime.UtcNow;
            this.Status = HealthcheckStatus.Healthy;
            this.HealthyMessage = string.Format(this.HealthyMessage, this.ServiceName);

            if (string.IsNullOrEmpty(this.ServiceName))
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Missing service name value",
                    Exception = null
                });

                return;
            }

            try
            {
                ServiceController service = new ServiceController(this.ServiceName);
                if(service.Status != ServiceControllerStatus.Running)
                {
                    this.Status = HealthcheckStatus.Error;
                    this.ErrorList.Entries.Add(new ErrorEntry
                    {
                        Created = DateTime.UtcNow,
                        Reason = string.Format("{0} service is now: {1}",this.ServiceName, service.Status),
                        Exception = null
                    });

                    return;
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
    }
}