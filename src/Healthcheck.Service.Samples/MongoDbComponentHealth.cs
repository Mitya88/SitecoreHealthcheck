namespace Healthcheck.Service.Samples
{
    using Healthcheck.Service.Customization;
    using MongoDB.Driver;
    using System;
    using System.Collections.Specialized;

    /// <summary>
    /// MongoDb sample health check
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Customization.CustomHealthcheckRun" />
    public class MongoDbComponentHealth : CustomHealthcheckRun
    {
        /// <summary>
        /// Does the healthcheck.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>
        /// The healthcheck result
        /// </returns>
        public override CustomHealthcheckResult DoHealthcheck(NameValueCollection parameters)
        {
            var connectionString = parameters["ConnectionString"];
            var result = new CustomHealthcheckResult
            {
                HealthyMessage = "Connection is ok",
                Status = HealthcheckStatus.Healthy
            };

            try
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    result.Status = HealthcheckStatus.Warning;
                    result.ErrorMessage = "Connectionstring is not provided in 'ConnectionString' param";
                    return result;
                }

                var client = new MongoClient(connectionString);

                if (client.Cluster.Description.State != MongoDB.Driver.Core.Clusters.ClusterState.Connected)
                {
                    result.Status = HealthcheckStatus.Error;
                    result.ErrorMessage = "State is disconnected";
                }
            }
            catch (Exception ex)
            {
                result.Status = HealthcheckStatus.Error;
                result.Exception = ex;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }
    }
}