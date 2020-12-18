using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Healthcheck.Service.Models
{
    public class ActiveSession
    {
        public string LastRequest { get; set; }

        public string Login { get; set; }

        public string UserName { get; set; }
    }
}