namespace Healthcheck.Service.Core
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;

    public class DatabaseCheck
    {
        public static HealthcheckResult RunHealthcheck(string connectionStringKey)
        {
            var checkResult = new HealthcheckResult
            {
                LastCheckTime = DateTime.UtcNow,
                Status = Customization.HealthcheckStatus.Healthy,
                HealthyMessage = "Database Connection is OK",
                ErrorList = new ErrorList
                {
                    Entries = new List<ErrorEntry>()
                }
            };

            if (string.IsNullOrEmpty(connectionStringKey))
            {
                checkResult.Status = HealthcheckStatus.Warning;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Missing connectionstring key",
                    Exception = null
                });

                return checkResult;
            }

            if (ConfigurationManager.ConnectionStrings[connectionStringKey] == null)
            {
                checkResult.Status = HealthcheckStatus.Warning;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Cannot find the connectionstring in the config",
                    Exception = null
                });

                return checkResult;
            }

            try
            {
                var connection = new SqlConnection(ConfigurationManager.ConnectionStrings[connectionStringKey].ConnectionString);
                connection.Open();
                connection.Close();
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