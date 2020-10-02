namespace Healthcheck.Service.Core.Messages
{
    using System;
    using System.Collections.Generic;

    public class OutGoingMessage
    {
        public string TargetInstance { get; set; }

        public Dictionary<string, string> Parameters { get; set; }

        public Guid ComponentId { get; set; }

        public DateTime EventRaised { get; set; }
    }
}