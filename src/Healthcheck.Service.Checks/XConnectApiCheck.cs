using Healthcheck.Service.Customization;
using Healthcheck.Service.Customization.Models;
using Sitecore.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace Healthcheck.Service.Core
{
    public class XConnectApiCheck
    {
        public static HealthcheckResult RunHealthcheck(string xConnectApiCertificateConnectionStringKey, string xConnectApiConnectionStringKey, int wWarnBefore)
        {
            var checkResult = new HealthcheckResult
            {
                LastCheckTime = DateTime.UtcNow,
                Status = Customization.HealthcheckStatus.Healthy,
                ErrorList = new ErrorList
                {
                    Entries = new List<ErrorEntry>()
                }
            };
            checkResult.LastCheckTime = DateTime.UtcNow;
            checkResult.Status = HealthcheckStatus.Healthy;

            if (!Settings.GetBoolSetting("Xdb.Enabled", false))
            {
                checkResult.Status = HealthcheckStatus.Warning;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Xdb is disabled",
                    Exception = null
                });

                return checkResult;
            }

            if (string.IsNullOrEmpty(xConnectApiCertificateConnectionStringKey) || string.IsNullOrEmpty(xConnectApiConnectionStringKey))
            {
                checkResult.Status = HealthcheckStatus.Warning;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "XConnect API Check is not configured correctly",
                    Exception = null
                });

                return checkResult;
            }

            if (ConfigurationManager.ConnectionStrings[xConnectApiConnectionStringKey] == null)
            {
                checkResult.Status = HealthcheckStatus.Warning;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Cannot find the XConnect API connectionstring in the config",
                    Exception = null
                });

                return checkResult;
            }

            if (ConfigurationManager.ConnectionStrings[xConnectApiCertificateConnectionStringKey] == null)
            {
                checkResult.Status = HealthcheckStatus.Warning;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Cannot find the XConnect API Certificate connectionstring in the config",
                    Exception = null
                });

                return checkResult;
            }

            try
            {
                var certificateConnectionstring = ConfigurationManager.ConnectionStrings[xConnectApiCertificateConnectionStringKey].ConnectionString.Split(';');
                NameValueCollection certDetails = new NameValueCollection();
                foreach (var values in certificateConnectionstring)
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

                    var cert = store.Certificates.Find((X509FindType)Enum.Parse(typeof(X509FindType), certDetails["FindType"]), certDetails["FindValue"], false);
                    if (cert == null || cert.Count == 0)
                    {
                        checkResult.Status = HealthcheckStatus.Error;
                        checkResult.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = DateTime.UtcNow,
                            Reason = string.Format("{0} certificate is missing from {1} {2} store", certDetails["FindValue"], certDetails["StoreName"], certDetails["StoreLocation"])
                        });
                    }
                    else if (cert.Count > 1)
                    {
                        checkResult.Status = HealthcheckStatus.Error;
                        checkResult.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = DateTime.UtcNow,
                            Reason = string.Format("{0} multiple certificate found from {1} {2} store", certDetails["FindValue"], certDetails["StoreName"], certDetails["StoreLocation"])
                        });
                    }
                    else
                    {
                        var certificate = cert[0];
                        var showExpirationWarning = certificate.NotAfter.AddDays(-wWarnBefore).Date <= DateTime.Now.Date;
                        checkResult.HealthyMessage = "Expiration: " + certificate.NotAfter.ToString("dd/MM/yyyy");

                        if (showExpirationWarning)
                        {
                            checkResult.Status = HealthcheckStatus.Warning;

                            if (certificate.NotAfter.Date < DateTime.Now.Date)
                            {
                                checkResult.Status = HealthcheckStatus.Error;
                                checkResult.ErrorList.Entries.Add(new ErrorEntry
                                {
                                    Created = DateTime.UtcNow,
                                    Reason = string.Format("{0} certificate expired", certDetails["FindValue"], (certificate.NotAfter - DateTime.Now).Days)
                                });
                            }
                            else
                            {
                                checkResult.ErrorList.Entries.Add(new ErrorEntry
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

                        var url = ConfigurationManager.ConnectionStrings[xConnectApiConnectionStringKey].ConnectionString.TrimEnd('/');
                        url = string.Format("{0}/healthz/live", url);
                        response = httpClient.GetAsync(url).Result;

                        if (response.StatusCode != HttpStatusCode.OK)
                        {
                            checkResult.Status = HealthcheckStatus.Error;
                            checkResult.ErrorList.Entries.Add(new ErrorEntry
                            {
                                Created = DateTime.UtcNow,
                                Reason = $"XConnect API returned with status code: {(int)response.StatusCode} - {response.ReasonPhrase}"
                            });
                        }
                    }
                    catch (Exception exception)
                    {
                        checkResult.Status = HealthcheckStatus.Error;
                        checkResult.ErrorList.Entries.Add(new ErrorEntry
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
                checkResult.Status = HealthcheckStatus.Error;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = exception.Message,
                    Exception = exception
                });
            }

            return checkResult;
        }
    }
}