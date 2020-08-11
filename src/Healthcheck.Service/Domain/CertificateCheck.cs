namespace Healthcheck.Service.Domain
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;

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
            this.LastCheckTime = DateTime.UtcNow;
            this.Status = HealthcheckStatus.Healthy;

            if (string.IsNullOrEmpty(StoreName) || string.IsNullOrEmpty(Location) || string.IsNullOrEmpty(Value) || string.IsNullOrEmpty(FindByType))
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Certificate Check is not configured correctly",
                    Exception = null
                });

                return;
            }

            try
            {
                var certificateList = new List<X509Certificate2>();
                var st = (StoreName)Enum.Parse(typeof(StoreName), this.StoreName);
                var store = new X509Store(st, (StoreLocation)Enum.Parse(typeof(StoreLocation), this.Location));
                var findByType = (X509FindType)Enum.Parse(typeof(X509FindType), this.FindByType);

                try
                {
                    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                    var cert = store.Certificates.Find(findByType, this.Value, false);
                    if (cert == null || cert.Count == 0)
                    {
                        this.Status = HealthcheckStatus.Error;
                        this.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = DateTime.UtcNow,
                            Reason = string.Format("{0} certificate is missing from {1} {2} {3} store", this.Value, this.StoreName, this.Location, this.FindByType)
                        });
                    }
                    else if (cert.Count > 1)
                    {
                        this.Status = HealthcheckStatus.Error;
                        this.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = DateTime.UtcNow,
                            Reason = string.Format("{0} multiple certificate found from {1} {2} {3} store", this.Value, this.StoreName, this.Location, this.FindByType)
                        });
                    }
                    else
                    {
                        var certificate = cert[0];
                        var showExpirationWarning = certificate.NotAfter.AddDays(-this.WarnBefore).Date <= DateTime.Now.Date;
                        this.HealthyMessage = "Expiration: " + certificate.NotAfter.ToString("dd/MM/yyyy");

                        if (showExpirationWarning)
                        {
                            this.Status = HealthcheckStatus.Warning;

                            if (certificate.NotAfter.Date < DateTime.Now.Date)
                            {
                                this.Status = HealthcheckStatus.Error;
                                this.ErrorList.Entries.Add(new ErrorEntry
                                {
                                    Created = DateTime.UtcNow,
                                    Reason = string.Format("{0} certificate expired", this.Value, (certificate.NotAfter - DateTime.Now).Days)
                                });
                            }
                            else
                            {
                                this.ErrorList.Entries.Add(new ErrorEntry
                                {
                                    Created = DateTime.UtcNow,
                                    Reason = string.Format("{0} certificate will expire in {1} days", this.Value, (certificate.NotAfter - DateTime.Now).Days)
                                });
                            }
                        }
                    }
                }
                finally
                {
                    store?.Close();
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