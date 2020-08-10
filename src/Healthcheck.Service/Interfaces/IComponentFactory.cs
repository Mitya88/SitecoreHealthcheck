namespace Healthcheck.Service.Interfaces
{
    using Healthcheck.Service.Domain;
    using Sitecore.Data.Items;

    /// <summary>
    /// Component factory interface
    /// </summary>
    public interface IComponentFactory
    {
        /// <summary>
        /// Creates the component.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        BaseComponent CreateComponent(Item item);
    }
}