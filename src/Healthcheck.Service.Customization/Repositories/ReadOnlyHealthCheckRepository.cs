namespace Healthcheck.Service.Customization.Repositories
{
    using Healthcheck.Service.Customization.Models;
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Globalization;
    using Sitecore.SecurityModel;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Health check state repository
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Customization.Repositories.IReadOnlyHealthCheckRepository" />
    public class ReadOnlyHealthCheckRepository : IReadOnlyHealthCheckRepository
    {  
        /// <summary>
        /// The components root folder identifier
        /// </summary>
        public const string ComponentsRootFolderId = "{AA16EF54-4954-4B4B-9056-537F97E9EE0F}";

        /// <summary>
        /// Gets the health check states.
        /// </summary>
        /// <returns>
        /// The healt check states
        /// </returns>
        public List<ComponentGroup> GetHealthCheckStates()
        {
            List<ComponentGroup> groups = new List<ComponentGroup>();
            using (new LanguageSwitcher(Language.Parse("en")))
            {
                using (new DatabaseSwitcher(Factory.GetDatabase("master")))
                {
                    using (new SecurityDisabler())
                    {
                        var componentsFolder = Sitecore.Context.Database.GetItem(new ID(ComponentsRootFolderId));

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
    }
}
