namespace Healthcheck.Service.Controllers
{
    using Healthcheck.Service.Customization.Models;
    using Healthcheck.Service.Interfaces;
    using Healthcheck.Service.Models;
    using Sitecore.Services.Infrastructure.Web.Http;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
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

            

           
            var data = new ApplicationInformation
            {
                IsAdministrator = Sitecore.Context.User.IsAdministrator,
                MemoryUsage = string.Format("{0} MB", currentProcess.WorkingSet64 / (1024 * 1024)),
                CpuTime = String.Format("{0:0.0}", GetCpuLoadAsync(1000) * 100)            
            };

            try
            {
                PerformanceCounter memProcess = new PerformanceCounter("Memory", "Available KBytes");
                data.AvailableMemory = string.Format("{0:0.0} MB", memProcess.NextValue() / (1024));
            }
            catch(Exception ex)
            {
                data.AvailableMemory = ex.Message;
            }

            try
            {
                long memKb;
                GetPhysicallyInstalledSystemMemory(out memKb);

                data.HardwareMemory = (memKb / 1024 / 1024).ToString();
            }
            catch(Exception ex)
            {
                data.HardwareMemory = ex.Message;
            }
            return data;
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

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

        /// <summary>
        /// Gets healthcheck for a component
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpGet]
        public ComponentHealth Component(string id)
        {
            this.healthcheckService.RunHealthcheck(id);
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

        double GetCpuLoadAsync(int MeasurementWindow)
        {
            Process CurrentProcess = Process.GetCurrentProcess();

            TimeSpan StartCpuTime = CurrentProcess.TotalProcessorTime;
            Stopwatch Timer = Stopwatch.StartNew();

            Thread.Sleep(1000);

            TimeSpan EndCpuTime = CurrentProcess.TotalProcessorTime;
            Timer.Stop();

            return (EndCpuTime - StartCpuTime).TotalMilliseconds / (Environment.ProcessorCount * Timer.ElapsedMilliseconds);
        }
    }
}