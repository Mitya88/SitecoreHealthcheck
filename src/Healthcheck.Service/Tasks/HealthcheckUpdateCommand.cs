namespace Healthcheck.Service.Tasks
{
    using Healthcheck.Service.Factories;
    using Healthcheck.Service.Repositories;
    using Healthcheck.Service.Services;
    using Healthcheck.Service.Tasks.Reports;
    using System;

    /// <summary>
    /// The behavior for the 'Healthcheck Update Command' Sitecore command.
    /// It does a healthcheck update for all components. Creates a report that is send by email.
    /// </summary>
    public class HealthcheckUpdateCommand
    {
        /// <summary>The execution of this command.</summary>
        public void Execute()
        {
            Sitecore.Diagnostics.Log.Info("[Sitecore.Healthcheck] Healthcheck Update started", this);

            var healthcheckService = new HealthcheckService(new ComponentFactory());
            healthcheckService.RunHealthcheck();

            Sitecore.Diagnostics.Log.Info("[Sitecore.Healthcheck] Healthcheck Update finished", this);

            Sitecore.Diagnostics.Log.Info("[Sitecore.Healthcheck] Send Email Report started", this);

            try
            {
                var healthcheckReport = new HealthcheckReport(new HealthcheckRepository(), new EmailService());
                healthcheckReport.SendEmailReport();
            }
            catch (Exception e)
            {
                Sitecore.Diagnostics.Log.Info("[Sitecore.Healthcheck] Error while sending the report", this);
                Sitecore.Diagnostics.Log.Info(e.Message, e.Source);
            }

            Sitecore.Diagnostics.Log.Info("[Sitecore.Healthcheck] Send Email Report finished", this);
        }
    }
}