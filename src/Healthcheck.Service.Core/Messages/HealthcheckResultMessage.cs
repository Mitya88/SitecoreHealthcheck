namespace Healthcheck.Service.Core.Messages
{
    using System;

    public class HealthcheckResultMessage
    {
        public HealthcheckResult Result { get; set; }

        public Guid ComponentId { get; set; }

        public DateTime LastCheckTime { get; set; }
    }
}