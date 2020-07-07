using Healthcheck.Service.Interfaces;
using Healthcheck.Service.Models;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Services.Infrastructure.Web.Http;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Healthcheck.Service.Controllers
{
    /// <summary>
    /// Controller for managing errors
    /// </summary>
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ErrorsApiController : ServicesApiController
    {
        /// <summary>
        /// The healthcheck repository
        /// </summary>
        private readonly IHealthcheckRepository healthcheckRepository;

        /// <summary>
        /// The healthcheck service
        /// </summary>
        private readonly IHealthcheckService healthcheckService;

        /// <summary>
        /// The healthcheck service
        /// </summary>
        private readonly IComponentFactory componentFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthcheckController" /> class.
        /// </summary>
        public ErrorsApiController(IHealthcheckRepository healthcheckRepository, IHealthcheckService healthcheckService, IComponentFactory componentFactory)
        {
            this.healthcheckRepository = healthcheckRepository;
            this.healthcheckService = healthcheckService;
            this.componentFactory = componentFactory;
        }

        /// <summary>
        /// Clear the errors entries.
        /// </summary>
        /// <remarks>Keeps the last error entry.</remarks>
        /// <returns>A list of all components.</returns>
        [HttpGet]
        public List<ComponentGroup> Clear()
        {
            var componentsGroups = this.healthcheckRepository.GetHealthcheck();

            foreach (var group in componentsGroups)
            {
                foreach (var componentHealth in group.Components)
                {
                    Item item;
                    using (new DatabaseSwitcher(Factory.GetDatabase(Constants.MasterDatabaseName)))
                    {
                        item = Sitecore.Context.Database.GetItem(new ID(componentHealth.Id));
                    }

                    if (item != null)
                    {
                        componentHealth.ClearButLastErrorEntry();
                        componentHealth.SaveComponent(item);
                    }
                } 
            }

            return componentsGroups;
        }
    }
}
