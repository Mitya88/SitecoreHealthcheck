namespace Healthcheck.Service.Domain
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Models;
    using Sitecore.Configuration;
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;

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
            this.LastCheckTime = DateTime.UtcNow;
            this.Status = HealthcheckStatus.Healthy;

            if (!Settings.GetBoolSetting("Xdb.Enabled", false))
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Xdb is disabled",
                    Exception = null
                });

                return;
            }

            if (string.IsNullOrEmpty(XConnectApiCertificateConnectionStringKey) || string.IsNullOrEmpty(XConnectApiConnectionStringKey))
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "XConnect API Check is not configured correctly",
                    Exception = null
                });

                return;
            }

            if (ConfigurationManager.ConnectionStrings[XConnectApiConnectionStringKey] == null)
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Cannot find the XConnect API connectionstring in the config",
                    Exception = null
                });

                return;
            }

            if (ConfigurationManager.ConnectionStrings[XConnectApiCertificateConnectionStringKey] == null)
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Cannot find the XConnect API Certificate connectionstring in the config",
                    Exception = null
                });

                return;
            }

            try
            {
                var certificateConnectionstring = ConfigurationManager.ConnectionStrings[XConnectApiCertificateConnectionStringKey].ConnectionString.Split(';');
                NameValueCollection certDetails = new NameValueCollection();
                foreach(var values in certificateConnectionstring)
                {
                    var parsed = values.Split('=');
                    certDetails.Add(parsed[0], parsed[1]);
                }

                var certificateList = new List<X509Certificate2>();
                var st = (StoreName)Enum.Parse(typeof(StoreName), certDetails["StoreName"]);
                var store = new X509Store(st, (StoreLocation)Enum.Parse(typeof(StoreLocation), certDetails["StoreLocation"]));
                try
                {
                    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                    var cert = store.Certificates.Find((X509FindType)Enum.Parse(typeof(X509FindType),certDetails["FindType"]), certDetails["FindValue"], false);
                    if (cert == null || cert.Count == 0)
                    {
                        this.Status = HealthcheckStatus.Error;
                        this.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = DateTime.UtcNow,
                            Reason = string.Format("{0} certificate is missing from {1} {2} store", certDetails["FindValue"], certDetails["StoreName"], certDetails["StoreLocation"])
                        });
                    }
                    else if (cert.Count > 1)
                    {
                        this.Status = HealthcheckStatus.Error;
                        this.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = DateTime.UtcNow,
                            Reason = string.Format("{0} multiple certificate found from {1} {2} store", certDetails["FindValue"], certDetails["StoreName"], certDetails["StoreLocation"])
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
                                    Reason = string.Format("{0} certificate expired", certDetails["FindValue"], (certificate.NotAfter - DateTime.Now).Days)
                                });
                            }
                            else
                            {
                                this.ErrorList.Entries.Add(new ErrorEntry
                                {
                                    Created = DateTime.UtcNow,
                                    Reason = string.Format("{0} certificate will expire in {1} days", certDetails["FindValue"], (certificate.NotAfter - DateTime.Now).Days)
                                });
                            }
                        }
                    }

                    HttpClient httpClient = new HttpClient();

                    try
                    {
                        HttpResponseMessage response = new HttpResponseMessage();

                        var certificate = cert[0];

                        WebRequestHandler handler = new WebRequestHandler();
                        handler.ClientCertificates.Add(certificate);

                        httpClient = new HttpClient(handler);

                        var url = ConfigurationManager.ConnectionStrings[XConnectApiConnectionStringKey].ConnectionString.TrimEnd('/');
                        url = string.Format("{0}/healthz/live", url);
                        response = httpClient.GetAsync(url).Result;

                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            this.Status = HealthcheckStatus.Error;
                            this.ErrorList.Entries.Add(new ErrorEntry
                            {
                                Created = DateTime.UtcNow,
                                Reason = $"XConnect API returned with status code: {(int)response.StatusCode} - {response.ReasonPhrase}"
                            });
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
                    finally
                    {
                        httpClient.Dispose();
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