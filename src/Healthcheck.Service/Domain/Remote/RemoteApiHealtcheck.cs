namespace Healthcheck.Service.Domain.Remote
{
    using Healthcheck.Service.Core;
    using Healthcheck.Service.Core.Messages;
    using Healthcheck.Service.Remote.EventQueue.Models.Event;
    using Microsoft.Azure.ServiceBus.Core;
    using Newtonsoft.Json;
    using Sitecore.Configuration;
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Specialized;
    using System.Text;

    /// <summary>API Healthcheck component</summary>
    /// <seealso cref="Healthcheck.Service.Domain.RemoteBaseComponent" />
    public class RemoteApiHealtcheck : RemoteBaseComponent
    {
        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the expected response code.
        /// </summary>
        /// <value>
        /// The expected response code.
        /// </value>
        public int ExpectedResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the expected response body.
        /// </summary>
        /// <value>
        /// The expected response body.
        /// </value>
        public string ExpectedResponseBody { get; set; }

        /// <summary>
        /// Gets or sets the post body.
        /// </summary>
        /// <value>
        /// The post body.
        /// </value>
        public string PostBody { get; set; }

        /// <summary>
        /// Gets or sets the request headers.
        /// </summary>
        /// <value>
        /// The request headers.
        /// </value>
        public NameValueCollection RequestHeaders { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the generate token URL.
        /// </summary>
        /// <value>
        /// The generate token URL.
        /// </value>
        public string GenerateTokenUrl { get; set; }

        /// <summary>
        /// Gets or sets the generate token endpoint method.
        /// </summary>
        /// <value>
        /// The generate token endpoint method.
        /// </value>
        public string GenerateTokenEndpointMethod { get; set; }

        /// <summary>
        /// Gets or sets the JWT token.
        /// </summary>
        /// <value>
        /// The JWT token.
        /// </value>
        public string JwtToken { get; set; }

        /// <summary>
        /// Gets or sets the name of the store.
        /// </summary>
        /// <value>
        /// The name of the store.
        /// </value>
        public string StoreName { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the "find by type".
        /// </summary>
        /// <value>
        /// The find by type.
        /// </value>
        public string FindByType { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The thumbprint.
        /// </value>
        public string Value { get; set; }

        private bool usingBasicAuthentication;

        private bool usingJwtAuthentication;

        private bool usingCertificateAuthentication;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteApiHealtcheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public RemoteApiHealtcheck(Item item) : base(item)
        {
            this.Url = item["API Url"];
            this.InnerItem = item;
            this.Method = item["Method"];
            this.ExpectedResponseCode = Sitecore.MainUtil.GetInt(item["Expected Response Code"], 200);
            this.ExpectedResponseBody = item["Expected Response Body"];
            this.PostBody = item["Post Body"];
            if (!string.IsNullOrEmpty(item["Request Headers"]))
            {
                this.RequestHeaders = Sitecore.StringUtil.GetNameValues(item["Request Headers"]);
            }

            if (item.HasChildren)
            {
                var autheticationMode = item.Children[0];
                if (autheticationMode.TemplateName == Constants.TemplateNames.BasicAuthenticationTemplateName)
                {
                    this.Username = autheticationMode["Username"];
                    this.Password = autheticationMode["Password"];

                    usingBasicAuthentication = true;
                }
                else if (autheticationMode.TemplateName == Constants.TemplateNames.JsonWebTokenAuthentication)
                {
                    this.Username = autheticationMode["Username"];
                    this.Password = autheticationMode["Password"];
                    this.GenerateTokenUrl = autheticationMode["Generate Token URL"];
                    this.JwtToken = autheticationMode["Token"];
                    this.GenerateTokenEndpointMethod = autheticationMode["Method"];

                    usingJwtAuthentication = true;
                }
                else if (autheticationMode.TemplateName == Constants.TemplateNames.CertificateAuthentication)
                {
                    this.StoreName = autheticationMode["StoreName"];
                    this.Location = autheticationMode["Location"];
                    this.FindByType = autheticationMode["FindByType"];
                    this.Value = autheticationMode["Value"];

                    usingCertificateAuthentication = true;
                }
            }
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            var dateTime = DateTime.UtcNow;
            this.SaveRemoteCheckStarted(dateTime);

            var message = new OutGoingMessage
            {
                Parameters = new System.Collections.Generic.Dictionary<string, string>
                {
                    {"Url", this.Url },
                    {"RequestHeaders",this.InnerItem    ["Request Headers"] },

                    {"Method", this.Method },
                     {"PostBody", this.PostBody},
                    {"ExpectedResponseCode", this.ExpectedResponseCode.ToString()},

                     {"ExpectedResponseBody", this.ExpectedResponseBody },
                    {"usingBasicAuthentication", this.usingBasicAuthentication.ToString() },
                    {"usingJwtAuthentication", this.usingJwtAuthentication.ToString() },
                     {"usingCertificateAuthentication", this.usingCertificateAuthentication.ToString() },
                    {"Username", this.Username },

                     {"Password", this.Password },
                    {"JwtToken", this.JwtToken },
                    {"GenerateTokenUrl", this.GenerateTokenUrl },
                     {"generateTokenEndpointMetho", this.GenerateTokenEndpointMethod },
                    {"storeName", this.StoreName },

                     {"location", this.Location },
                    {"findByTypeName", this.FindByType },
                    {"value", this.Value }
                },
                TargetInstance = this.TargetInstance,
                ComponentId = this.InnerItem.ID.Guid,
                EventRaised = dateTime
            };

            if (Settings.GetSetting("Healthcheck.Remote.Mode").Equals("eventqueue", StringComparison.OrdinalIgnoreCase))
            {
                var remoteEvent = new HealthcheckStartedRemoteEvent(Constants.TemplateNames.RemoteApiHealthcheckTemplateName, "healthcheck:started:remote", message);
                var database = Sitecore.Configuration.Factory.GetDatabase("web");
                var eventQueue = database.RemoteEvents.EventQueue;
                eventQueue.QueueEvent<HealthcheckStartedRemoteEvent>(remoteEvent, true, false);
            }
            else
            {
                var messageSender = new MessageSender(SharedConfig.ConnectionStringOrKey, SharedConfig.TopicName);
                var busMessage = new Microsoft.Azure.ServiceBus.Message
                {
                    ContentType = "application/json",
                    Label = Constants.TemplateNames.RemoteApiHealthcheckTemplateName,
                    Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message))
                };

                messageSender.SendAsync(busMessage).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
    }
}