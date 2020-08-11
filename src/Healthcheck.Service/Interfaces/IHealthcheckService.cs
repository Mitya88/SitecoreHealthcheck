namespace Healthcheck.Service.Interfaces
{
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