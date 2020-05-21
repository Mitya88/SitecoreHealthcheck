namespace Healthcheck.Service.Services
{
    using Healthcheck.Service.Domain;
    using Healthcheck.Service.Interfaces;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Globalization;
    using Sitecore.SecurityModel;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// Healthcheck service
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Interfaces.IHealthcheckService" />
    public class HealthcheckService : IHealthcheckService
    {
        /// <summary>
        /// The component factory
        /// </summary>
        private readonly IComponentFactory componentFactory;

        /// <summary>
        /// The default number of days to keep logs
        /// </summary>
        private const int DefaultNumberOfDaysToKeepLogs = 5;

        /// <summary>
        /// The maximum number of threads settings key
        /// </summary>
        private string maxNumberOfThreadsSettingsKey = "Healthcheck.MaxNumberOfThreads";

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthcheckService"/> class.
        /// </summary>
        /// <param name="componentFactory">The component factory.</param>
        public HealthcheckService(IComponentFactory componentFactory)
        {
            this.componentFactory = componentFactory;
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void RunHealthcheck(string id = null)
        {
            using (new LanguageSwitcher(Language.Parse("en")))
            {
                var maximumNumberOfThreads = Settings.GetIntSetting(maxNumberOfThreadsSettingsKey, 1);

                var queue = new ConcurrentQueue<BaseComponent>();

                using (new DatabaseSwitcher(Factory.GetDatabase("master")))
                {
                    using (new SecurityDisabler())
                    {
                        var settingsItem = Sitecore.Context.Database.GetItem(new ID(Constants.SettingsItemId));

                        int numberOfDaysToKeepLogs = 0;

                        if (!int.TryParse(settingsItem["Days"], out numberOfDaysToKeepLogs))
                        {

                            numberOfDaysToKeepLogs = DefaultNumberOfDaysToKeepLogs;
                        }

                        var componentsFolder = Sitecore.Context.Database.GetItem(new ID(Constants.ComponentsRootFolderId));

                        foreach (Item item in componentsFolder.Axes.GetDescendants())
                        {
                            if (!string.IsNullOrEmpty(id) && !item.ID.ToString().Equals(id, System.StringComparison.OrdinalIgnoreCase))
                            {
                                continue;
                            }

                            var component = componentFactory.CreateComponent(item);

                            if (component != null)
                            {
                                queue.Enqueue(component);
                            }
                        }

                        List<Action> actions = new List<Action>();

                        for (int i = 0; i < maximumNumberOfThreads; i++)
                        {
                            Action action = () =>
                            {
                                BaseComponent component;
                                while (queue.TryDequeue(out component))
                                {
                                    component.RunHealthcheck();
                                    component.SaveHealthcheckResult(numberOfDaysToKeepLogs);
                                }
                            };

                            actions.Add(action);
                        }

                        Parallel.Invoke(actions.ToArray());
                    }
                }
            }
        }
    }
}