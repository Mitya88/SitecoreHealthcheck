namespace Healthcheck.Service.Tasks
{
    using Healthcheck.Service.Factories;
    using Healthcheck.Service.Repositories;
    using Healthcheck.Service.Services;
    using Healthcheck.Service.Tasks.Reports;
    using Sitecore.Data.Items;
    using System;
    using System.Linq;

    /// <summary>
    /// The behavior for the 'Healthcheck Update Command' Sitecore command.
    /// It does a healthcheck update for all components. Creates a report that is send by email.
    /// </summary>
    public class HealthcheckUpdateCommand
    {
        /// <summary>
        /// The execution of this command.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="command">The command.</param>
        /// <param name="schedule">The schedule.</param>
        public void Execute(Item[] items, Sitecore.Tasks.CommandItem command, Sitecore.Tasks.ScheduleItem schedule)
        {
            Sitecore.Diagnostics.Log.Info("[Sitecore.Healthcheck] Healthcheck Update started", this);

            var healthcheckService = new HealthcheckService(new ComponentFactory());


            if(items!= null && items.Length >0)
            {
                foreach(var item in items)
                {
                    healthcheckService.RunHealthcheck(item.ID.ToString());
                }
            }
            else
            {
                healthcheckService.RunHealthcheck();
            }

            Sitecore.Diagnostics.Log.Info("[Sitecore.Healthcheck] Healthcheck Update finished", this);

            Sitecore.Diagnostics.Log.Info("[Sitecore.Healthcheck] Send Email Report started", this);

            try
            {
                var components = new HealthcheckRepository().GetHealthcheck();

                if (items != null && items.Length > 0)
                {
                    foreach(var component in components)
                    {
                        component.Components = component.Components.Where(t => items.Any(p => p.ID.ToString().Equals(t.Id, StringComparison.OrdinalIgnoreCase))).ToList();
                    }

                    components = components.Where(t => t.Components != null && t.Components.Any()).ToList();
                }

                var healthcheckReport = new HealthcheckReport(new EmailService());
                healthcheckReport.SendEmailReport(components);
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