namespace Healthcheck.Service.Controllers
{
    using Healthcheck.Service.Interfaces;
    using Sitecore.Services.Infrastructure.Web.Http;
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
        /// Initializes a new instance of the <see cref="HealthcheckController" /> class.
        /// </summary>
        public HealthcheckErrorsApiController(IHealthcheckRepository healthcheckRepository)
        {
            this.healthcheckRepository = healthcheckRepository;
        }

        /// <summary>
        /// Clear the errors entries.
        /// </summary>
        /// <remarks>Keeps the last error entry.</remarks>
        /// <returns>A list of all components.</returns>
        [HttpGet]
        public IHttpActionResult Clear()
        {
            if (!Sitecore.Context.User.IsAdministrator)
            {
                return this.Unauthorized();
            }

            this.healthcheckRepository.ClearComponentsErrorsButLast();
            var componentsGroups = this.healthcheckRepository.GetHealthcheck();

            return this.Ok(componentsGroups);
        }
    }
}