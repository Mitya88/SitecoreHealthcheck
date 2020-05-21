namespace Healthcheck.Service.Tasks.Reports
{
    using System.Text;
    using Healthcheck.Service.Extensions;
    using Healthcheck.Service.Interfaces;
    using Healthcheck.Service.Models;
    using Sitecore.Globalization;
    using Sitecore.Web;

    /// <summary>Creates the healthcheck report.</summary>
    public class HealthcheckReport
    {
        private readonly IHealthcheckRepository healthcheckRepository;
        private readonly IEmailService emailService;
        private StringBuilder mailBodyBuilder;

        /// <summary>Initializes a new instance of the <see cref="HealthcheckReport"/> class.</summary>
        /// <param name="healthcheckRepository">The healthcheck repository.</param>
        /// <param name="emailService">The email service.</param>
        public HealthcheckReport(IHealthcheckRepository healthcheckRepository, IEmailService emailService)
        {
            this.healthcheckRepository = healthcheckRepository;
            this.emailService = emailService;
            mailBodyBuilder = new StringBuilder();
        }

        /// <summary>Sends the email report.</summary>
        public void SendEmailReport()
        {
            using (new LanguageSwitcher(Language.Parse("en")))
            {
                var body = GenerateReportBody();
                var emailSettings = new SettingsModel();

                emailService.SendEmail(emailSettings.SenderEmail, emailSettings.RecipientEmails, emailSettings.Subject, body);
            }
        }

        /// <summary>Generates the report body.</summary>
        /// <returns>Report body as HTML.</returns>
        private string GenerateReportBody()
        {

            string header = "<p> This is a summary of components after Healtcheck: </p>";
            mailBodyBuilder.AppendLine().AppendLine(header);

            mailBodyBuilder.AppendLine("<table>");

            CreateTableHeader();

            AddTableData();

            mailBodyBuilder.AppendLine($"</table style=\"{Styles.table}\">").AppendLine();

            string footer = string.Format("<p> End of report. <a href=\"{0}\">Open the healthcheck application</a></p>", WebUtil.GetFullUrl("/sitecore/shell/client/Applications/healthcheck/"));
            mailBodyBuilder.AppendLine().AppendLine(footer);

            return mailBodyBuilder.ToString();
        }

        /// <summary>Adds the table data.</summary>
        private void AddTableData()
        {
            var settingsModel = new SettingsModel();
            foreach (var componentsGroup in healthcheckRepository.GetHealthcheck())
            {
                var groupName = componentsGroup.GroupName;

                mailBodyBuilder.AppendLine("<tr>");
                mailBodyBuilder.Append($"<td colspan=\"4\" style=\"{Styles.tdGroup}\">{groupName}</td>");
                mailBodyBuilder.AppendLine("</tr>");

                foreach (var componentHealth in componentsGroup.Components)
                {
                    if (componentHealth.IsValidForReporting(settingsModel))
                    {
                        CreateTableRow(componentHealth);
                    }
                }
            }
        }

        /// <summary>Creates the table header.</summary>
        private void CreateTableHeader()
        {
            mailBodyBuilder.AppendLine("<tr>");

            mailBodyBuilder.Append($"<th style=\"{Styles.th}\">Component name</th>");
            mailBodyBuilder.Append($"<th style=\" {Styles.th}\">Last check</th>");
            mailBodyBuilder.Append($"<th style=\"{Styles.th}\">Status</th>");
            mailBodyBuilder.Append($"<th style=\"{Styles.th}\">Message</th>");

            mailBodyBuilder.AppendLine("</tr>");
        }

        /// <summary>Creates the table row.</summary>
        /// <param name="componentHealth">The component health.</param>
        private void CreateTableRow(ComponentHealth componentHealth)
        {
            // Add report table row
            var message = componentHealth.ErrorCount > 0 && componentHealth.Status != Customization.HealthcheckStatus.Healthy ? componentHealth.ErrorList.Entries[0].Reason : componentHealth.HealthyMessage;
            var rowStyle = GetRowStyle(componentHealth.Status.ToString());
            mailBodyBuilder.AppendLine($"<tr style=\"{rowStyle}\">");

            mailBodyBuilder.Append($"<td style=\"{Styles.td}\">{componentHealth.Name}</td>");
            mailBodyBuilder.Append($"<td style=\"{Styles.td}\">{componentHealth.LastCheckTime}</td>");
            mailBodyBuilder.Append($"<td style=\"{Styles.td}\">{componentHealth.Status}</td>");
            mailBodyBuilder.Append($"<td style=\"{Styles.td}\">{message}</td>");

            mailBodyBuilder.AppendLine("</tr>");
        }

        private string GetRowStyle(string status)
        {
            if (status == "Healthy")
            {
                return Styles.trHealthy;
            }
            else if (status == "Warning")
            {
                return Styles.trWarining;
            }

            return Styles.trError;
        }
    }

    public struct Styles
    {
        internal const string table = "border: 1px solid black; border-collapse: collapse; width: 100%;";
        internal const string th = "border: 1px solid black; border-collapse: collapse; padding: 5px; text-align: left;";
        internal const string tdGroup = "border: 1px solid black; border-collapse: collapse; padding: 5px; background-color: #ccc;";
        internal const string td = "border: 1px solid black; border-collapse: collapse; padding: 5px;";
        internal const string trError = "background-color: #F00;";
        internal const string trWarining = "background-color: #FFA500;";
        internal const string trHealthy = "background-color: #008000;";
    }
}