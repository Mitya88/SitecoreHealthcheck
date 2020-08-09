namespace Healthcheck.Service.Controllers
{
    using Healthcheck.Service.Interfaces;
    using Healthcheck.Service.Models;
    using Sitecore.Services.Infrastructure.Web.Http;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Cors;

    /// <summary>
    /// Controller for managing errors
    /// </summary>
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HealthcheckErrorsApiController : ServicesApiController
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
        public HealthcheckErrorsApiController(IHealthcheckRepository healthcheckRepository, IHealthcheckService healthcheckService, IComponentFactory componentFactory)
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
            this.healthcheckRepository.ClearComponentsErrorsButLast();
            var componentsGroups = this.healthcheckRepository.GetHealthcheck();
            
            return componentsGroups;
        }
    }
}
