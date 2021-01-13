using System;

namespace Healthcheck.Service.Models
{
    public class LastErrorResponse
    {
        public string ComponentName { get; set; }

        public string Message { get; set; }

        public DateTime Date { get; set; }
    }
}