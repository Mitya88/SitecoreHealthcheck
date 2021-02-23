namespace Healthcheck.Service.Domain
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using Healthcheck.Service.Interfaces;
    using Healthcheck.Service.Services;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Data.Items;
    using Sitecore.DependencyInjection;
    using System;

    /// <summary>
    /// Application Insight healtcheck component
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.BaseComponent" />
    public class ApplicationInsightHealthcheck : BaseComponent
    {
        /// <summary>
        /// Gets or sets the application identifier.
        /// </summary>
        /// <value>
        /// The application identifier.
        /// </value>
        public string ApplicationID { get; set; }

        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        /// <value>
        /// The API key.
        /// </value>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the number of days to check.
        /// </summary>
        /// <value>
        /// The number of days to check.
        /// </value>
        public int NumberOfDaysToCheck { get; set; }

        /// <summary>
        /// The application insights service
        /// </summary>
        private readonly IApplicationInsightsService applicationInsightsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInsightHealthcheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public ApplicationInsightHealthcheck(Item item) : base(item)
        {
            this.ApplicationID = item["Application ID"];
            this.ApiKey = item["API Key"];
            this.NumberOfDaysToCheck = Sitecore.MainUtil.GetInt(item["Number of Days to Check"], 0);

            this.applicationInsightsService = new ApplicationInsightsService();
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            this.LastCheckTime = DateTime.UtcNow;
            this.Status = HealthcheckStatus.Healthy;
            this.HealthyMessage = "Application insights contains no errors";
            this.ErrorList.Entries.Clear();

            if (string.IsNullOrEmpty(ApplicationID) || string.IsNullOrEmpty(ApiKey))
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Application Insight Check is not configured correctly",
                    Exception = null
                });

                return;
            }

            try
            {
                var result = applicationInsightsService.GetSitecoreLogs(ApplicationID, ApiKey, NumberOfDaysToCheck);

                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    this.Status = HealthcheckStatus.Warning;
                    this.ErrorList.Entries.Add(new ErrorEntry
                    {
                        Created = DateTime.UtcNow,
                        Reason = "Application Insight Check request error: " + result.StatusCode,
                        Exception = null
                    });

                    return;
                }

                var warnings = result.GetWarnings();

                var errors = result.GetErrors();

                if (errors.Count > 0)
                {
                    this.Status = HealthcheckStatus.Error;
                    foreach (var error in errors)
                    {
                        this.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = error.Timestamp,
                            Reason = error.CustomDimensions.LoggerName + " " + error.Trace.Message,
                            Exception = null
                        });
                    }
                }
                else if (warnings.Count > 0)
                {
                    this.Status = HealthcheckStatus.Warning;
                    foreach (var warning in warnings)
                    {
                        this.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = warning.Timestamp,
                            Reason = warning.CustomDimensions.LoggerName + " " + warning.Trace.Message,
                            Exception = null
                        });
                    }
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