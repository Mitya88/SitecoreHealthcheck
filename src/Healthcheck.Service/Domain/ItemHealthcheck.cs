namespace Healthcheck.Service.Domain
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Data.Managers;
    using Sitecore.Globalization;
    using Sitecore.SecurityModel;
    using System;
    using System.Linq;

    /// <summary>
    /// Item healthcheck component
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.BaseComponent" />
    public class ItemHealthcheck : BaseComponent
    {
        /// <summary>
        /// Gets or sets the item identifier or query.
        /// </summary>
        /// <value>
        /// The item identifier or query.
        /// </value>
        public string ItemIdOrQuery { get; set; }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>
        /// The language.
        /// </value>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public string Database { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemHealthcheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public ItemHealthcheck(Item item) : base(item)
        {
            this.ItemIdOrQuery = item["Item"];
            this.Language = item["Language"];
            this.Database = item["Database"];
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            this.LastCheckTime = DateTime.UtcNow;
            this.Status = HealthcheckStatus.Healthy;
            this.HealthyMessage = string.Format("{0} item is exists in {1} database, in {2} language", this.ItemIdOrQuery, this.Database, this.Language);
            if (string.IsNullOrEmpty(ItemIdOrQuery) || string.IsNullOrEmpty(Language) || string.IsNullOrEmpty(Database))
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Item Check is not configured correctly",
                    Exception = null
                });

                return;
            }

            try
            {
                using (new SecurityDisabler())
                {
                    using (new LanguageSwitcher(LanguageManager.GetLanguage(this.Language)))
                    {
                        using (new DatabaseSwitcher(Factory.GetDatabase(this.Database)))
                        {
                            if (ItemIdOrQuery.StartsWith("/"))
                            {
                                var items = Sitecore.Context.Database.SelectItems(ItemIdOrQuery);

                                if (items == null || !items.Any())
                                {
                                    this.Status = HealthcheckStatus.Error;
                                    this.ErrorList.Entries.Add(new ErrorEntry
                                    {
                                        Created = DateTime.UtcNow,
                                        Reason = string.Format("{0} is missing from {1} database, in {2} language", this.ItemIdOrQuery, this.Database, this.Language)
                                    });
                                }
                            }
                            else
                            {
                                var item = Sitecore.Context.Database.GetItem(new ID(this.ItemIdOrQuery));

                                if (item == null)
                                {
                                    this.Status = HealthcheckStatus.Error;
                                    this.ErrorList.Entries.Add(new ErrorEntry
                                    {
                                        Created = DateTime.UtcNow,
                                        Reason = string.Format("{0} is missing from {1} database, in {2} language", this.ItemIdOrQuery, this.Database, this.Language)
                                    });

                                    return;
                                }

                                if (!item.Languages.Any(t => t.Name == this.Language))
                                {
                                    this.Status = HealthcheckStatus.Error;
                                    this.ErrorList.Entries.Add(new ErrorEntry
                                    {
                                        Created = DateTime.UtcNow,
                                        Reason = string.Format("{0} is missing from {1} database, in {2} language", this.ItemIdOrQuery, this.Database, this.Language)
                                    });
                                }
                            }
                        }
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