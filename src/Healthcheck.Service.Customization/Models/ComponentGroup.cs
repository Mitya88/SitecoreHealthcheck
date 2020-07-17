namespace Healthcheck.Service.Customization.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Component group model
    /// </summary>
    public class ComponentGroup
    {
        /// <summary>
        /// Gets or sets the name of the group.
        /// </summary>
        /// <value>
        /// The name of the group.
        /// </value>
        public string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the components.
        /// </summary>
        /// <value>
        /// The components.
        /// </value>
        public List<ComponentHealth> Components { get; set; }
    }
}