namespace Healthcheck.Service.Models
{
    using System;
    using System.Collections.Generic;

    public class ErrorCountReport
    {
        public List<DateTime> Dates { get; set; }

        public List<int> Counts { get; set; }
    }
}