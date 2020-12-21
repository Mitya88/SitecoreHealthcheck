namespace Healthcheck.Service.Controllers
{
    using Healthcheck.Service.Customization.Models;
    using Healthcheck.Service.Interfaces;
    using Healthcheck.Service.Models;
    using Healthcheck.Service.Utilities;
    using Sitecore;
    using Sitecore.Configuration;
    using Sitecore.ContentSearch;
    using Sitecore.ContentSearch.SearchTypes;
    using Sitecore.Services.Infrastructure.Web.Http;
    using Sitecore.Web.Authentication;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web.Http;
    using System.Web.Http.Cors;

    /// <summary>
    /// Entity controller for item commander
    /// </summary>
    /// <seealso cref="Sitecore.Services.Infrastructure.Sitecore.Services.EntityService{Healthcheck.Service.Models.ItemResponse}" />
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HealthcheckApiController : ServicesApiController
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
        /// Initializes a new instance of the <see cref="HealthcheckController" /> class.
        /// </summary>
        /// <param name="healthcheckRepository">The healthcheck repository.</param>
        /// <param name="healthcheckServic">The healthcheck service.</param>
        public HealthcheckApiController(IHealthcheckRepository healthcheckRepository, IHealthcheckService healthcheckService)
        {
            this.healthcheckRepository = healthcheckRepository;
            this.healthcheckService = healthcheckService;
        }

        /// <summary>
        /// Determines whether this instance is ok.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ApplicationInformation AppInfo()
        {
            var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            var cpu = HardwareUtil.GetCpuLoadAsync(1000) * 100;
            var data = new ApplicationInformation
            {
                IsAdministrator = Sitecore.Context.User.IsAdministrator
            };

            return data;
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<ComponentGroup> Run()
        {
            this.healthcheckService.RunHealthcheck();
            return this.LimitErrorEntries(this.healthcheckRepository.GetHealthcheck());
        }

        /// <summary>
        /// Gets the healthcheck components.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public List<ComponentGroup> Get()
        {
            var data = this.healthcheckRepository.GetHealthcheck();
            return this.LimitErrorEntries(this.healthcheckRepository.GetHealthcheck());
        }

        [HttpGet]
        public List<IndexDetailResponse> GetIndexDetails()
        {
            return this.healthcheckRepository.GetIndexes();
        }

        [HttpGet]
        public ErrorCountReport GetErrorCounts()
        {
            return this.healthcheckRepository.GetErrorCountReport();
        }

        [HttpGet]
        public CacheStatisticsResponse GetCacheStatistics()
        {
            return this.healthcheckRepository.GetCacheStatistics();
        }

        [HttpGet]
        public List<DataFolderResponse> GetDataFolderStatistics()
        {
            return this.healthcheckRepository.GetDataFolderStatistics();
        }

        [HttpGet]
        public MemoryUsageResponse GetMemoryUsage()
        {
            return this.healthcheckRepository.GetMemoryUsage();
        }

        [HttpGet]
        public ComponentStatisticsResponse GetComponentStatistics()
        {
            return this.healthcheckRepository.GetComponentStatistics();
        }

        [HttpGet]
        public CpuTimeResponse GetCpuTime()
        {
            return this.healthcheckRepository.GetCpuTime();
        }

        [HttpGet]
        public DriveInfoResponse GetDriveInfo()
        {
            return this.healthcheckRepository.GetDriveInfo();
        }

        [HttpGet]
        public object GetActiveUsers()
        {
            List<DomainAccessGuard.Session> sessions = DomainAccessGuard.Sessions;
            List<DomainAccessGuard.Session> list = sessions.Where<DomainAccessGuard.Session>((Func<DomainAccessGuard.Session, bool>)(guardSession => string.Compare(guardSession.UserName, Context.User.Name, StringComparison.InvariantCultureIgnoreCase) == 0 || Context.IsAdministrator || Settings.AllowLogoutOfAllUsers)).ToList<DomainAccessGuard.Session>();
           
            int count = list.Count;
            var result = list.Select<DomainAccessGuard.Session, ActiveSession>(new Func<DomainAccessGuard.Session, ActiveSession>(t => new ActiveSession()
            {
                UserName = t.UserName,
                LastRequest = DateUtil.ToServerTime(t.LastRequest).ToString(),
                Login = DateUtil.ToServerTime(t.Created).ToString()
            })).ToList();

            return result;
        }
        [HttpGet]
        public string Memory()
        {
            List<string> data = new List<string>();

            for(long i = 0; i < 1000000000000; i++)
            {
                data.Add(Guid.NewGuid().ToString());
            }
            return "ok";
        }

        /// <summary>
        /// Gets healthcheck for a component
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public ComponentHealth Component(string id, bool onlyState = false)
        {
            if (!onlyState)
            {
                this.healthcheckService.RunHealthcheck(id);
            }
            
            return this.LimitErrorEntries(this.healthcheckRepository.GetHealthcheck(id));
        }

        /// <summary>
        /// Returns the error messages in csv
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>Csv</returns>
        [HttpGet]
        public HttpResponseMessage Csv(string id, string componentName)
        {
            string fileName = string.Format("Export_{0}_{1}.csv", componentName, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(this.healthcheckRepository.GenerateErrorReport(id));
            writer.Flush();
            stream.Position = 0;

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = fileName };
            return result;
        }

        /// <summary>
        /// Returns the error messages in csv
        /// </summary>
        /// <returns>
        /// Csv
        /// </returns>
        [HttpGet]
        public HttpResponseMessage ExportState()
        {
            string fileName = string.Format("StateExport_{0}.csv", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(this.healthcheckRepository.GenerateReport());
            writer.Flush();
            stream.Position = 0;

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = fileName };
            return result;
        }

        /// <summary>
        /// Limits the error entries.
        /// </summary>
        /// <param name="groups">The groups.</param>
        /// <returns>Filtered groups</returns>
        private List<ComponentGroup> LimitErrorEntries(List<ComponentGroup> groups)
        {
            foreach (var group in groups)
            {
                foreach (var component in group.Components)
                {
                    component.ErrorList.Entries = component.ErrorList.Entries.OrderByDescending(t => t.Created).Take(100).ToList();
                }
            }

            return groups;
        }

        /// <summary>
        /// Limits the error entries.
        /// </summary>
        /// <param name="component">The component.</param>
        /// <returns>
        /// Filtered component
        /// </returns>
        private ComponentHealth LimitErrorEntries(ComponentHealth component)
        {
            component.ErrorList.Entries = component.ErrorList.Entries.OrderByDescending(t => t.Created).Take(100).ToList();

            return component;
        }
    }
}