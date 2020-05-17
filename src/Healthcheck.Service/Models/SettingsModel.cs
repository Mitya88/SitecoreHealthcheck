namespace Healthcheck.Service.Models
{
    using Healthcheck.Service.Customization;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using System;

    /// <summary>
    /// Maps the item Settings.
    /// </summary>
    public sealed class SettingsModel
    {
        /// <summary>Gets or sets the name of the sender.</summary>
        /// <value>The name of the sender.</value>
        public string SenderName { get; set; }

        /// <summary>Gets or sets the sender email.</summary>
        /// <value>The sender email.</value>
        public string SenderEmail { get; set; }

        /// <summary>Gets or sets the recipient emails.</summary>
        /// <value>The recipient emails.</value>
        public string RecipientEmails { get; set; }

        /// <summary>Gets or sets the subject.</summary>
        /// <value>The subject.</value>
        public string Subject { get; set; }

        /// <summary>Gets or sets the error level.</summary>
        /// <value>The error level.</value>
        public HealthcheckStatus ErrorLevel { get; set; }
        
        /// <summary>Initializes a new instance of the <see cref="SettingsModel"/> class.</summary>
        public SettingsModel()
        {
            Item settingsItem = null;

            using (new DatabaseSwitcher(Factory.GetDatabase("master")))
            {
                settingsItem = Sitecore.Context.Database.GetItem(new ID(Constants.SettingsItemId));
            }

            SenderName = settingsItem.Fields["Sender Name"].HasValue ? settingsItem.Fields["Sender Name"].Value : string.Empty;
            SenderEmail = settingsItem.Fields["Sender Email"].HasValue ? settingsItem.Fields["Sender Email"].Value : string.Empty;
            RecipientEmails = settingsItem.Fields["Recipient Emails"].HasValue ? settingsItem.Fields["Recipient Emails"].Value : string.Empty;
            Subject = settingsItem.Fields["Subject"].HasValue ? settingsItem.Fields["Subject"].Value : string.Empty;

            if (!string.IsNullOrEmpty(settingsItem["Error Level"]))
            {
                this.ErrorLevel = (HealthcheckStatus)Enum.Parse(typeof(HealthcheckStatus), settingsItem["Error Level"]);
            }
            else
            {
                this.ErrorLevel = HealthcheckStatus.UnKnown;
            }
        }
    }
}