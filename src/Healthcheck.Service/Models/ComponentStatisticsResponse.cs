namespace Healthcheck.Service.Models
{
    public class ComponentStatisticsResponse
    {
        public int HealthyCount { get; set; }

        public int WarningCount { get; set; }

        public int ErrorCount { get; set; }

        public int UnknownCount { get; set; }
    }
}