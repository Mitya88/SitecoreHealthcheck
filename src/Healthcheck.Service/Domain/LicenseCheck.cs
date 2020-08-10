namespace Healthcheck.Service.Domain
{
    using System;
    using System.Xml;
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using Healthcheck.Service.Models;
    using Sitecore.Configuration;
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
            this.LastCheckTime = DateTime.UtcNow;
            this.Status = HealthcheckStatus.Healthy;

            try {
                var licenseFile = Settings.LicenseFile;
                XmlDocument doc = new XmlDocument();
                doc.Load(licenseFile);
                var expirationNodeList = doc.GetElementsByTagName("expiration");
                var expirationDate = DateTime.ParseExact(expirationNodeList[0].InnerText, "yyyyMMddTHHmmss", System.Globalization.CultureInfo.CurrentCulture);

                if (expirationDate.AddDays(-WarnBefore).Date <= DateTime.Now.Date && expirationDate.AddDays(-ErrorBefore).Date > DateTime.UtcNow.Date)
                {
                    this.Status = HealthcheckStatus.Warning;
                    this.ErrorList.Entries.Add(new ErrorEntry
                    {
                        Created = DateTime.UtcNow,
                        Reason = string.Format("License will expire in {0} days.", (expirationDate - DateTime.UtcNow).Days),
                        Exception = null
                    });

                    return;
                }

                if (expirationDate.AddDays(-ErrorBefore).Date <= DateTime.UtcNow.Date)
                {
                    this.Status = HealthcheckStatus.Error;
                    this.ErrorList.Entries.Add(new ErrorEntry
                    {
                        Created = DateTime.UtcNow,
                        Reason = string.Format("License will expire in {0} days.", (expirationDate - DateTime.UtcNow).Days),
                        Exception = null
                    });

                    return;
                }
            }
            catch(Exception exception)
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