using Healthcheck.Service.Customization;
using Healthcheck.Service.Customization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcheck.Service.Checks
{
    public class HealthcheckResult
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public HealthcheckStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the error messages.
        /// </summary>
        /// <value>
        /// The error messages.
        /// </value>
        public ErrorList ErrorList { get; set; }

        /// <summary>
        /// Gets or sets the last check time.
        /// </summary>
        /// <value>
        /// The last check time.
        /// </value>
        public DateTime LastCheckTime { get; set; }

        /// <summary>
        /// Gets or sets the healthy message.
        /// </summary>
        /// <value>
        /// The healthy message.
        /// </value>
        public string HealthyMessage { get; set; }
    }
}
