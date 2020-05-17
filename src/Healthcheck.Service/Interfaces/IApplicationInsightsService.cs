namespace Healthcheck.Service.Interfaces
{
    using Healthcheck.Service.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Interface for ApplicationInsightService
    /// </summary>
    public interface IApplicationInsightsService
    {
        /// <summary>
        /// Gets the Sitecore logs.
        /// </summary>
        /// <param name="appid">The Application Insight application ID.</param>
        /// <param name="apikey">The Application Insight API key.</param>
        /// <param name="numberOfDays">The number of days to check.</param>
        /// <returns></returns>
        ApplicationInsightResult GetSitecoreLogs(string appid, string apikey, int numberOfDays);
    }
}