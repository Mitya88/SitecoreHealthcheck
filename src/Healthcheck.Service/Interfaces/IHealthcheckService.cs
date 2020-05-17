namespace Healthcheck.Service.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// Healthcheck service interface
    /// </summary>
    public interface IHealthcheckService
    {
        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        /// <param name="id">The identifier.</param>
        void RunHealthcheck(string id = null);
    }
}