namespace Healthcheck.Service.Core
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;

    public class CertificateCheck
    {
        public static HealthcheckResult RunHealthcheck(string storeName, string location, string value, string findByTypeName, int warnBefore)
        {
            var checkResult = new HealthcheckResult
            {
                LastCheckTime = DateTime.UtcNow,
                Status = Customization.HealthcheckStatus.Healthy,
                HealthyMessage = "Log files contain no errors",
                ErrorList = new ErrorList
                {
                    Entries = new List<ErrorEntry>()
                }
            };

            if (string.IsNullOrEmpty(storeName) || string.IsNullOrEmpty(location) || string.IsNullOrEmpty(value) || string.IsNullOrEmpty(findByTypeName))
            {
                checkResult.Status = HealthcheckStatus.Warning;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Certificate Check is not configured correctly",
                    Exception = null
                });

                return checkResult;
            }

            try
            {
                var certificateList = new List<X509Certificate2>();
                var st = (StoreName)Enum.Parse(typeof(StoreName), storeName);
                var store = new X509Store(st, (StoreLocation)Enum.Parse(typeof(StoreLocation), location));
                var findByType = (X509FindType)Enum.Parse(typeof(X509FindType), findByTypeName);

                try
                {
                    store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                    var cert = store.Certificates.Find(findByType, value, false);
                    if (cert == null || cert.Count == 0)
                    {
                        checkResult.Status = HealthcheckStatus.Error;
                        checkResult.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = DateTime.UtcNow,
                            Reason = string.Format("{0} certificate is missing from {1} {2} {3} store", value, storeName, location, findByTypeName)
                        });
                    }
                    else if (cert.Count > 1)
                    {
                        checkResult.Status = HealthcheckStatus.Error;
                        checkResult.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = DateTime.UtcNow,
                            Reason = string.Format("{0} multiple certificate found from {1} {2} {3} store", value, storeName, location, findByTypeName)
                        });
                    }
                    else
                    {
                        var certificate = cert[0];
                        var showExpirationWarning = certificate.NotAfter.AddDays(-warnBefore).Date <= DateTime.Now.Date;
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
                                    Reason = string.Format("{0} certificate expired", value, (certificate.NotAfter - DateTime.Now).Days)
                                });
                            }
                            else
                            {
                                checkResult.ErrorList.Entries.Add(new ErrorEntry
                                {
                                    Created = DateTime.UtcNow,
                                    Reason = string.Format("{0} certificate will expire in {1} days", value, (certificate.NotAfter - DateTime.Now).Days)
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