using System;

namespace Healthcheck.Service.Models
{
    public class UpcomingExpirations
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public DateTime Date { get; set; }
    }
}