namespace Healthcheck.Service.Core
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using System;
    using System.Collections.Generic;
    using System.ServiceProcess;

    public class WindowsServiceCheck
    {
        public static HealthcheckResult RunHealthcheck(string serviceName, string healtyMessage)
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
            checkResult.HealthyMessage = string.Format(healtyMessage, serviceName);

            if (string.IsNullOrEmpty(serviceName))
            {
                checkResult.Status = HealthcheckStatus.Warning;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Missing service name value",
                    Exception = null,
                    ErrorLevel = ErrorLevel.Warning
                });

                return checkResult;
            }

            try
            {
                ServiceController service = new ServiceController(serviceName);
                if (service.Status != ServiceControllerStatus.Running)
                {
                    checkResult.Status = HealthcheckStatus.Error;
                    checkResult.ErrorList.Entries.Add(new ErrorEntry
                    {
                        Created = DateTime.UtcNow,
                        Reason = string.Format("{0} service is now: {1}", serviceName, service.Status),
                        Exception = null,
                        ErrorLevel = ErrorLevel.Error

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
                    Exception = exception,
                    ErrorLevel = ErrorLevel.Error
                });
            }

            return checkResult;
        }
    }
}