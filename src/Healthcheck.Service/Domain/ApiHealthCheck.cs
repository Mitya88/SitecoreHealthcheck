namespace Healthcheck.Service.Domain
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using Healthcheck.Service.Models;
    using Newtonsoft.Json;
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Web;

    /// <summary>
    /// API Healthcheck component
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.BaseComponent" />
    public class ApiHealthCheck : BaseComponent
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
        /// Initializes a new instance of the <see cref="ApiHealthCheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public ApiHealthCheck(Item item) : base(item)
        {
            this.Url = item["API Url"];
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
            this.LastCheckTime = DateTime.UtcNow;
            this.Status = Customization.HealthcheckStatus.Healthy;
            this.HealthyMessage = "API endpoint looks OK";

            if (string.IsNullOrEmpty(Url))
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Missing API URL",
                    Exception = null
                });

                return;
            }

            HttpClient httpClient = new HttpClient();

            try
            {
                HttpResponseMessage response = new HttpResponseMessage();

                httpClient = AddAuthentication(httpClient);
                
                if (RequestHeaders != null && RequestHeaders.Count > 0)
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    foreach (string headerKey in RequestHeaders)
                    {
                        httpClient.DefaultRequestHeaders.Add(headerKey, RequestHeaders[headerKey]);
                    }
                }

                if (Method == "POST")
                {
                    HttpContent content = new StringContent(PostBody);

                    response = httpClient.PostAsync(Url, content).Result;
                }
                else
                {
                    response = httpClient.GetAsync(Url).Result;
                }

                if ((int)response.StatusCode != ExpectedResponseCode)
                {
                    this.Status = HealthcheckStatus.Error;
                    this.ErrorList.Entries.Add(new ErrorEntry
                    {
                        Created = DateTime.UtcNow,
                        Reason = $"API returned with status code: {(int)response.StatusCode} - {response.ReasonPhrase}"
                    });
                }
                else if (!string.IsNullOrEmpty(ExpectedResponseBody))
                {
                    var responseBody = CleanText(response.Content.ReadAsStringAsync().Result);

                    if (!responseBody.Equals(CleanText(ExpectedResponseBody), StringComparison.OrdinalIgnoreCase))
                    {
                        this.Status = HealthcheckStatus.Error;
                        this.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = DateTime.UtcNow,
                            Reason = $"API returned with an unexpected response body"
                        }); ;
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
            finally
            {
                httpClient.Dispose();
            }
        }

        /// <summary>
        /// Adds an authentication method.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        private HttpClient AddAuthentication(HttpClient httpClient)
        {
            if (usingBasicAuthentication)
            {
                var byteArray = Encoding.ASCII.GetBytes($"{Username}:{Password}");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
            else if (usingJwtAuthentication)
            {
                string accessToken = JwtToken;

                if (string.IsNullOrEmpty(accessToken))
                {
                    accessToken = GetToken(Username, Password, GenerateTokenUrl);
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            else if (usingCertificateAuthentication)
            {
                var certificate = GetCertificate();

                if (certificate == null)
                {
                    throw new ArgumentException("Certificate can't be found");
                }

                WebRequestHandler handler = new WebRequestHandler();
                handler.ClientCertificates.Add(certificate);

                httpClient = new HttpClient(handler);
            }

            return httpClient;
        }

        /// <summary>
        /// Gets the JWT token.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="generateTokenUrl">The generate token URL.</param>
        /// <returns></returns>
        private string GetToken(string username, string password, string generateTokenUrl)
        {
            string token = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                if (GenerateTokenEndpointMethod == "POST")
                {
                    var userModel = new UserModel()
                    {
                        Username = username,
                        Password = password
                    };

                    string stringData = JsonConvert.SerializeObject(userModel);
                    var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage response = client.PostAsync(generateTokenUrl, contentData).Result;
                    token = response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    var uriBuilder = new UriBuilder(generateTokenUrl);
                    var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                    query["username"] = username;
                    query["password"] = password;
                    uriBuilder.Query = query.ToString();

                    token = client.GetStringAsync(uriBuilder.Uri).Result;
                }
            }

            return token;
        }

        /// <summary>
        /// Cleans the text.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        private string CleanText(string result)
        {
            var resultText = result.Replace("\r\n", string.Empty).Replace(" ", string.Empty);

            return resultText;
        }

        /// <summary>
        /// Gets the certificate.
        /// </summary>
        /// <returns></returns>
        private X509Certificate2 GetCertificate()
        {
            X509Certificate2 cert = null;
            var st = (StoreName)Enum.Parse(typeof(StoreName), this.StoreName);
            var store = new X509Store(st, (StoreLocation)Enum.Parse(typeof(StoreLocation), this.Location));
            var findByType = (X509FindType)Enum.Parse(typeof(X509FindType), this.FindByType);

            try
            {
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                X509Certificate2Collection certCollection = store.Certificates.Find
                (
                    findByType,
                    this.Value,
                    false
                );

                if (certCollection.Count > 0)
                {
                    cert = certCollection[0];
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
            finally
            {
                store?.Close();
            }

            return cert;
        }

        /// <summary>
        /// User model
        /// </summary>
        private class UserModel
        {
            public string Username { get; set; }

            public string Password { get; set; }
        }
    }
}