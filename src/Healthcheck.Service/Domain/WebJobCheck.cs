namespace Healthcheck.Service.Domain
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Models;
    using Sitecore.Data.Items;
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    /// <summary>
    /// Web Job healthcheck component
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.BaseComponent" />
    public class WebJobCheck : BaseComponent
    {
        /// <summary>
        /// Gets or sets the hook URL.
        /// </summary>
        /// <value>
        /// The hook URL.
        /// </value>
        public string HookUrl { get; set; }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="WebJobCheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public WebJobCheck(Item item) : base(item)
        {
            this.HookUrl = item["Hook Url"];
            this.UserName = item["User Name"];
            this.Password = item["Password"];
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            this.LastCheckTime = DateTime.UtcNow;
            this.Status = HealthcheckStatus.Healthy;

            if (string.IsNullOrEmpty(this.HookUrl) || string.IsNullOrEmpty(this.UserName) || string.IsNullOrWhiteSpace(this.Password))
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Webjob is not configured correctly.",
                    Exception = null
                });

                return;
            }

            try
            {
                var request = WebRequest.Create(this.HookUrl);

                request.ContentType = "application/json";
                request.Method = "GET";

                var encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(this.UserName + ":" + this.Password));
                request.Headers.Add("Authorization", "Basic " + encoded);

                using (var response = request.GetResponse())
                {
                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        var data = sr.ReadToEnd();
                        var webJobResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<WebJobResponse>(data);

                        if (!webJobResponse.Status.Equals("running", StringComparison.OrdinalIgnoreCase))
                        {
                            this.Status = HealthcheckStatus.Error;
                            this.ErrorList.Entries.Add(new ErrorEntry
                            {
                                Created = DateTime.UtcNow,
                                Reason = string.Format("The current state: {0}", webJobResponse.Status)
                            });
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this.Status = HealthcheckStatus.Error;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = exception.Message,
                    Exception = exception
                });
            }
        }
    }
}