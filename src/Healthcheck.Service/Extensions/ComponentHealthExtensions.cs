namespace Healthcheck.Service.Extensions
{
    using Healthcheck.Service.Models;

    /// <summary>ComponentHealth extension methods.</summary>
    public static class ComponentHealthExtensions
    {
        /// <summary>Determines whether the componentHealth is valid for reporting.</summary>
        /// <param name="componentHealth">The component health.</param>
        /// <returns>
        ///   <c>true</c> if [is valid for reporting] [the specified component health]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidForReporting(this ComponentHealth componentHealth, SettingsModel settingsModel)
        {
            if (componentHealth.Status == Customization.HealthcheckStatus.Error && settingsModel.ErrorLevel == Customization.HealthcheckStatus.Error)
            {
                return true;
            }
            else if ((componentHealth.Status == Customization.HealthcheckStatus.Warning || componentHealth.Status == Customization.HealthcheckStatus.Error) && settingsModel.ErrorLevel == Customization.HealthcheckStatus.Error)
            {
                return true;
            }
            else if (settingsModel.ErrorLevel == Customization.HealthcheckStatus.Healthy)
            {
                return true;
            }
            return false;
        }
    }
}