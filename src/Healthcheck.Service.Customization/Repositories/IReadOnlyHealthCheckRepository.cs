namespace Healthcheck.Service.Customization.Repositories
{
    using Healthcheck.Service.Customization.Models;
    using System.Collections.Generic;

    /// <summary>
    /// Health check state repository
    /// </summary>
    public interface IReadOnlyHealthCheckRepository
    {
        /// <summary>
        /// Gets the health check states.
        /// </summary>
        /// <returns>The healt check states</returns>
        List<ComponentGroup> GetHealthCheckStates();
    }
}
