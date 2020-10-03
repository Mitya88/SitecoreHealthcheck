using Healthcheck.Service.Core.Messages;
using Healthcheck.Service.Customization;
using Healthcheck.Service.Customization.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace Healthcheck.Service.Core
{
    public class ApiCheck
    {

        public static HealthcheckResult RunHealthcheck(OutGoingMessage message)
        {
            
            return RunHealthcheck(message.Parameters["Url"], HttpUtility.ParseQueryString(message.Parameters["RequestHeaders"]), message.Parameters["Method"], message.Parameters["PostBody"], int.Parse(message.Parameters["ExpectedResponseCode"]), message.Parameters["ExpectedResponseBody"], bool.Parse(message.Parameters["usingBasicAuthentication"]), bool.Parse(message.Parameters["usingJwtAuthentication"]), bool.Parse(message.Parameters["usingCertificateAuthentication"]), message.Parameters["Username"], message.Parameters["Password"], message.Parameters["JwtToken"],message.Parameters["GenerateTokenUrl"], message.Parameters["generateTokenEndpointMetho"],message.Parameters["storeName"], message.Parameters["location"], message.Parameters["findByTypeName"], message.Parameters["value"]);
        }

        public static HealthcheckResult RunHealthcheck(string Url, NameValueCollection RequestHeaders, string Method, string PostBody, int ExpectedResponseCode, string ExpectedResponseBody, bool usingBasicAuthentication, bool usingJwtAuthentication, bool usingCertificateAuthentication, string Username, string Password, string JwtToken, string GenerateTokenUrl, string generateTokenEndpointMetho, string storeName, string location, string findByTypeName, string value)
        {
            var checkResult = new HealthcheckResult
            {
                LastCheckTime = DateTime.UtcNow,
                Status = Customization.HealthcheckStatus.Healthy,
                HealthyMessage = "API endpoint looks OK",
                ErrorList = new ErrorList
                {
                    Entries = new List<ErrorEntry>()
                }
            };

            if (string.IsNullOrEmpty(Url))
            {
                checkResult.Status = HealthcheckStatus.Warning;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Missing API URL",
                    Exception = null
                });

                return checkResult;
            }

            HttpClient httpClient = new HttpClient();

            try
            {
                HttpResponseMessage response = new HttpResponseMessage();

                httpClient = AddAuthentication(httpClient, usingBasicAuthentication, usingJwtAuthentication, usingCertificateAuthentication, Username, Password, JwtToken, GenerateTokenUrl, generateTokenEndpointMetho, storeName, location, findByTypeName, value);

                if (RequestHeaders != null && RequestHeaders.Count > 0)
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    foreach (string headerKey in RequestHeaders.Keys)
                    {
                        httpClient.DefaultRequestHeaders.Add(headerKey, RequestHeaders[headerKey].ToString());
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
                    checkResult.Status = HealthcheckStatus.Error;
                    checkResult.ErrorList.Entries.Add(new ErrorEntry
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
                        checkResult.Status = HealthcheckStatus.Error;
                        checkResult.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = DateTime.UtcNow,
                            Reason = $"API returned with an unexpected response body"
                        }); ;
                    }
                }
            }
            catch (Exception exception)
            {
                checkResult.Status = HealthcheckStatus.Error;
                checkResult.ErrorList.Entries.Add(new ErrorEntry
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

            return checkResult;
        }

        /// <summary>
        /// Adds an authentication method.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        private static HttpClient AddAuthentication(HttpClient httpClient, bool usingBasicAuthentication, bool usingJwtAuthentication, bool usingCertificateAuthentication, string Username, string Password, string JwtToken, string GenerateTokenUrl, string generateTokenEndpointMetho, string storeName, string location, string findByTypeName, string value)
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
                    accessToken = GetToken(Username, Password, GenerateTokenUrl, generateTokenEndpointMetho);
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            else if (usingCertificateAuthentication)
            {
                var certificate = GetCertificate(storeName, location, findByTypeName, value);

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
        private static string GetToken(string username, string password, string generateTokenUrl, string generateTokenEndpointMethod)
        {
            string token = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                if (generateTokenEndpointMethod == "POST")
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
        private static string CleanText(string result)
        {
            var resultText = result.Replace("\r\n", string.Empty).Replace(" ", string.Empty);

            return resultText;
        }

        /// <summary>
        /// Gets the certificate.
        /// </summary>
        /// <returns></returns>
        private static X509Certificate2 GetCertificate(string storeName, string location, string findByTypeName, string value)
        {
            X509Certificate2 cert = null;
            var st = (StoreName)Enum.Parse(typeof(StoreName), storeName);
            var store = new X509Store(st, (StoreLocation)Enum.Parse(typeof(StoreLocation), location));
            var findByType = (X509FindType)Enum.Parse(typeof(X509FindType), findByTypeName);

            try
            {
                store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);

                X509Certificate2Collection certCollection = store.Certificates.Find
                (
                    findByType,
                    value,
                    false
                );

                if (certCollection.Count > 0)
                {
                    cert = certCollection[0];
                }
            }
            catch (Exception exception)
            {
                return null;
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