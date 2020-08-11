namespace Healthcheck.Service.Domain
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using Sitecore.Data.Items;
    using System;
    using System.Collections.Specialized;
    using System.Reflection;

    /// <summary>
    /// Custom healthcheck component
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.BaseComponent" />
    public class CustomHealthcheck : BaseComponent
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>
        /// The parameters.
        /// </value>
        public NameValueCollection Parameters { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificateCheck"/> class.
        /// </summary>
        /// <param name="item">The item.</param>
        public CustomHealthcheck(Item item) : base(item)
        {
            this.Type = item["Type"];
            var parameters = item["Parameters"];
            this.Parameters = Sitecore.Web.WebUtil.ParseUrlParameters(parameters);
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            this.LastCheckTime = DateTime.UtcNow;
            string assemblyName = string.Empty;
            string typeName = string.Empty;

            try
            {
                assemblyName = this.Type.Split(',')[1].Trim();
                typeName = this.Type.Split(',')[0].Trim();
            }
            catch (Exception)
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = string.Format("{0} could not be parsed, use the following format: <NameSpace>.<Class>,<AssemblyName>", this.Type)
                });

                return;
            }

            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(assemblyName);
            }
            catch (Exception ex)
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = string.Format("{0} assembly could not be loaded, please check the configuration!", assemblyName),
                    Exception = ex
                });

                return;
            }

            Type type = assembly.GetType(typeName);

            if (type == null)
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = string.Format("{0} type could not be loaded, please check the configuration!", typeName)
                });

                return;
            }

            var methodName = "DoHealthcheck";

            MethodInfo methodInfo = type.GetMethod(methodName);

            if (methodInfo == null)
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = string.Format("{0} method could not be loaded, please make sure you implemented the 'DoHealthcheck' method!", methodName)
                });

                return;
            }

            try
            {
                CustomHealthcheckResult result = null;
                ParameterInfo[] parameters = methodInfo.GetParameters();
                object classInstance = Activator.CreateInstance(type, null);

                object[] parametersArray = new object[] { this.Parameters };

                result = methodInfo.Invoke(classInstance, parametersArray) as CustomHealthcheckResult;

                if (result != null)
                {
                    this.Status = result.Status;
                    this.HealthyMessage = result.HealthyMessage;
                    if (this.Status != HealthcheckStatus.Healthy)
                    {
                        this.ErrorList.Entries.Add(new ErrorEntry
                        {
                            Created = DateTime.UtcNow,
                            Reason = result.ErrorMessage,
                            Exception = result.Exception
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                this.Status = HealthcheckStatus.Error;
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = ex.Message,
                    Exception = ex
                });
            }
        }
    }
}