namespace Healthcheck.Service.Factories
{
    using Healthcheck.Service.Core;
    using Healthcheck.Service.Domain;
    using Healthcheck.Service.Interfaces;
    using Sitecore.Data.Items;

    /// <summary>
    /// Factory for creating components
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Interfaces.IComponentFactory" />
    public class ComponentFactory : IComponentFactory
    {
        /// <summary>
        /// Creates the component.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public BaseComponent CreateComponent(Item item)
        {
            if (item.TemplateName.Equals(Constants.TemplateNames.DatabaseHealthcheckTemplateName))
            {
                return new DatabaseHealthCheck(item);
            }
            else if (item.TemplateName.Equals(Constants.TemplateNames.ItemHealthcheckTemplateName))
            {
                return new ItemHealthcheck(item);
            }
            else if (item.TemplateName.Equals(Constants.TemplateNames.CertificateCheckTemplateName))
            {
                return new CertificateCheck(item);
            }
            else if (item.TemplateName.Equals(Constants.TemplateNames.LogFileCheckTemplateName))
            {
                return new LogFileHealthcheck(item);
            }
            else if (item.TemplateName.Equals(Constants.TemplateNames.ApplicationInsightTemplateName))
            {
                return new ApplicationInsightHealthcheck(item);
            }
            else if (item.TemplateName.Equals(Constants.TemplateNames.CustomHealthcheckTemplateName))
            {
                return new CustomHealthcheck(item);
            }
            else if (item.TemplateName.Equals(Constants.TemplateNames.ApiHealthCheckTemplateName))
            {
                return new ApiHealthCheck(item);
            }
            else if (item.TemplateName.Equals(Constants.TemplateNames.SearchIndexHealthCheckTemplateName))
            {
                return new SearchIndexHealthCheck(item);
            }
            else if (item.TemplateName.Equals(Constants.TemplateNames.LicenseCheckTemplateName))
            {
                return new LicenseCheck(item);
            }
            else if (item.TemplateName.Equals(Constants.TemplateNames.XConnectApiCheckTemplateName))
            {
                return new XConnectApiCheck(item);
            }
            else if (item.TemplateName.Equals(Constants.TemplateNames.WindowsServiceCheckTemplateName))
            {
                return new WindowsServiceCheck(item);
            }
            else if (item.TemplateName.Equals(Constants.TemplateNames.WebJobCheckTemplateName))
            {
                return new WebJobCheck(item);
            }
            else if (item.TemplateName.Equals(Constants.TemplateNames.KeepAliveTemplateName))
            {
                return new KeepAliveCheck(item);
            }
            else if (item.TemplateName.Equals(Constants.TemplateNames.QueueCheckTemplateName))
            {
                return new QueueCheck(item);
            }
            else if (item.TemplateName.Equals(Constants.TemplateNames.LocalDiskSpaceCheckTemplateName))
            {
                return new LocalDiskSpaceCheck(item);
            }
            else if (item.TemplateName.Equals(Constants.TemplateNames.SPECheckTemplateName))
            {
                return new SPECheck(item);
            }
            else if (item.TemplateName.Equals(Constants.TemplateNames.RemoteLogFileCheckTemplateName))
            {
                return new RemoteLogFileCheck(item);
            }

            return null;
        }
    }
}