namespace Healthcheck.Service.Interfaces
{
    using Healthcheck.Service.Customization.Models;
    using Healthcheck.Service.Models;
    using System.Collections.Generic;

    /// <summary>
    /// Healthcheck repository interface
    /// </summary>
    public interface IHealthcheckRepository
    {
        /// <summary>
        /// Gets the healthcheck.
        /// </summary>
        /// <returns></returns>
        List<ComponentGroup> GetHealthcheck();

        /// <summary>
        /// Gets the healthcheck.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        ComponentHealth GetHealthcheck(string id);

        /// <summary>
        /// Generates the error report.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>
        /// the error report
        /// </returns>
        string GenerateErrorReport(string id);

        /// <summary>
        /// Generates the report.
        /// </summary>
        /// <returns>The current state report</returns>
        string GenerateReport();
    }
}