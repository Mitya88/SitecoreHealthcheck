namespace Healthcheck.Service.Services
{
    using Healthcheck.Service.Interfaces;
    using Healthcheck.Service.Models;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Service class for Application Insight
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Interfaces.IApplicationInsightsService" />
    public class ApplicationInsightsService : IApplicationInsightsService
    {
        /// <summary>
        /// The Application Insight API URL
        /// </summary>
        private const string URL = "https://api.applicationinsights.io/v1/apps/{0}/{1}/{2}?{3}";

        /// <summary>
        /// Gets the Sitecore logs.
        /// </summary>
        /// <param name="appid">The Application Insight application ID.</param>
        /// <param name="apikey">The Application Insight API key.</param>
        /// <param name="numberOfDays">The number of days to check.</param>
        /// <returns></returns>
        public ApplicationInsightResult GetSitecoreLogs(string appid, string apikey, int numberOfDays)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-api-key", apikey);

            StringBuilder parametersBuilder = new StringBuilder();
            parametersBuilder.Append($"timespan=P{numberOfDays}D");
            parametersBuilder.Append("&$filter=" + HttpUtility.UrlEncode("trace/severityLevel eq 2 or trace/severityLevel eq 3"));
            parametersBuilder.Append("&$orderby=" + HttpUtility.UrlEncode("timestamp desc"));
            parametersBuilder.Append("&$select=" + HttpUtility.UrlEncode("timestamp, customDimensions/LoggerName, trace/message, trace/severityLevel"));
            parametersBuilder.Append("&$top=5000");

            var req = string.Format(URL, appid, "events", "traces", parametersBuilder.ToString());

            HttpResponseMessage response = client.GetAsync(req).Result;

            var appInsightResult = new ApplicationInsightResult();
            appInsightResult.StatusCode = response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;

                appInsightResult = JsonConvert.DeserializeObject<ApplicationInsightResult>(result);
                appInsightResult.StatusCode = response.StatusCode;

                return appInsightResult;
            }

            return appInsightResult;
        }
    }
}