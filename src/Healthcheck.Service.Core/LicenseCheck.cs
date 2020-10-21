namespace Healthcheck.Service.Core
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using Sitecore.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Xml;

    public class LicenseCheck
    {
        public static HealthcheckResult RunHealthcheck(int warnBefore, int errorBefore)
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

            try
            {
                var licenseFile = Settings.LicenseFile;
                XmlDocument doc = new XmlDocument();
                doc.Load(licenseFile);
                var expirationNodeList = doc.GetElementsByTagName("expiration");
                var expirationDate = DateTime.ParseExact(expirationNodeList[0].InnerText, "yyyyMMddTHHmmss", System.Globalization.CultureInfo.CurrentCulture);

                if (expirationDate.AddDays(-warnBefore).Date <= DateTime.Now.Date && expirationDate.AddDays(-errorBefore).Date > DateTime.UtcNow.Date)
                {
                    checkResult.Status = HealthcheckStatus.Warning;
                    checkResult.ErrorList.Entries.Add(new ErrorEntry
                    {
                        Created = DateTime.UtcNow,
                        Reason = string.Format("License will expire in {0} days.", (expirationDate - DateTime.UtcNow).Days),
                        Exception = null
                    });

                    return checkResult;
                }

                if (expirationDate.AddDays(-errorBefore).Date <= DateTime.UtcNow.Date)
                {
                    checkResult.Status = HealthcheckStatus.Error;
                    checkResult.ErrorList.Entries.Add(new ErrorEntry
                    {
                        Created = DateTime.UtcNow,
                        Reason = string.Format("License will expire in {0} days.", (expirationDate - DateTime.UtcNow).Days),
                        Exception = null
                    });

                    return checkResult;
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