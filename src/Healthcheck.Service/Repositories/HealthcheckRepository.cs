namespace Healthcheck.Service.Repositories
{
    using Healthcheck.Service.Domain;
    using Healthcheck.Service.Interfaces;
    using Healthcheck.Service.Models;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Globalization;
    using Sitecore.SecurityModel;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Healthcheck repository
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Interfaces.IHealthcheckRepository" />
    public class HealthcheckRepository : IHealthcheckRepository
    {
        /// <summary>
        /// Gets the healthcheck.
        /// </summary>
        /// <returns></returns>
        public List<ComponentGroup> GetHealthcheck()
        {
            List<ComponentGroup> groups = new List<ComponentGroup>();
            using (new LanguageSwitcher(Language.Parse("en")))
            {
                using (new DatabaseSwitcher(Factory.GetDatabase("master")))
                {
                    using (new SecurityDisabler())
                    {
                        List<BaseComponent> components = new List<BaseComponent>();
                        var componentsFolder = Sitecore.Context.Database.GetItem(new ID(Constants.ComponentsRootFolderId));

                        foreach (Item componentGroup in componentsFolder.Children)
                        {
                            var group = new ComponentGroup();
                            group.GroupName = componentGroup["Name"];
                            group.Components = componentGroup.Children.Select(t => new ComponentHealth(t)).ToList();
                            groups.Add(group);
                        }
                    }
                }
            }

            return groups.Where(t => t.Components.Any()).ToList();
        }

        /// <summary>
        /// Gets the healthcheck for a component.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ComponentHealth GetHealthcheck(string id)
        {
            List<ComponentGroup> groups = new List<ComponentGroup>();
            using (new LanguageSwitcher(Language.Parse("en")))
            {
                using (new DatabaseSwitcher(Factory.GetDatabase("master")))
                {
                    using (new SecurityDisabler())
                    {
                        var component = Sitecore.Context.Database.GetItem(new ID(id));

                        return new ComponentHealth(component);
                    }
                }
            }
        }

        /// <summary>
        /// Generates the error report.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// the error report
        /// </returns>
        public string GenerateErrorReport(string id)
        {
            using (new LanguageSwitcher(Language.Parse("en")))
            {
                using (new DatabaseSwitcher(Factory.GetDatabase("master")))
                {
                    using (new SecurityDisabler())
                    {
                        var component = Sitecore.Context.Database.GetItem(new ID(id));

                        var componentHealth = new ComponentHealth(component);

                        StringBuilder sb = new StringBuilder();

                        sb.AppendLine("Created,Reason,Exception");

                        foreach (var error in componentHealth.ErrorList.Entries.OrderByDescending(t => t.Created))
                        {
                            sb.AppendLine(string.Format("{0},{1},{2}",
                                EscapeSpecialCharacters(error.Created.ToString()),
                                 EscapeSpecialCharacters(error.Reason),
                                EscapeSpecialCharacters(error.Exception != null ? error.Exception.ToString() : string.Empty)));
                        }

                        return sb.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Generates the error report.
        /// </summary>
        /// <returns>
        /// the error report
        /// </returns>
        public string GenerateReport()
        {
            using (new LanguageSwitcher(Language.Parse("en")))
            {
                using (new DatabaseSwitcher(Factory.GetDatabase("master")))
                {
                    using (new SecurityDisabler())
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.AppendLine("Group,Component,Status,LastCheckTime,Message");

                        foreach (var group in this.GetHealthcheck())
                        {
                            foreach (var component in group.Components)
                            {
                                sb.AppendLine(string.Format("{0},{1},{2},{3},{4}",
                                    EscapeSpecialCharacters(group.GroupName),
                                    EscapeSpecialCharacters(component.Name),
                                    EscapeSpecialCharacters(component.Status.ToString()),
                                    EscapeSpecialCharacters(component.LastCheckTime.ToString()),
                                    EscapeSpecialCharacters(component.Status == Customization.HealthcheckStatus.Healthy ? component.HealthyMessage : component.ErrorList.Entries.OrderByDescending(t => t.Created).FirstOrDefault().Reason)));
                            }
                        }

                        return sb.ToString();
                    }
                }
            }
        }

        private string EscapeSpecialCharacters(string input)
        {
            return input.Replace(",", " ")
                        .Replace(System.Environment.NewLine, " ");
        }
    }
}