namespace Healthcheck.Service.Core.Messages
{
    using System;
    using System.Collections.Generic;

    public class HealthcheckResultMessage
    {
        public HealthcheckResult Result { get; set; }

        public Guid ComponentId { get; set; }

        public DateTime LastCheckTime { get; set; }
    }
}
