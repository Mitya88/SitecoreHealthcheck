namespace Healthcheck.Service.Customization
{
    using System.Collections.Specialized;

    /// <summary>
    /// Custom healthcheck run class
    /// </summary>
    public abstract class CustomHealthcheckRun
    {
        /// <summary>
        /// Does the healthcheck.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>The healthcheck result</returns>
        public abstract CustomHealthcheckResult DoHealthcheck(NameValueCollection parameters);
    }
}