namespace Healthcheck.Service.Domain
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using Sitecore.Data.Fields;
    using Sitecore.Data.Items;
    using Spe.Core.Host;
    using System;
    using System.Collections;
    using System.Collections.Specialized;

    /// <summary>
    /// SPECheck component
    /// </summary>
    /// <seealso cref="Healthcheck.Service.Domain.BaseComponent" />
    public class SPECheck : BaseComponent
    {
        /// <summary>
        /// Gets or sets the ScriptItem.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public Item ScriptItem { get; set; }

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
        public SPECheck(Item item) : base(item)
        {
            if (!string.IsNullOrEmpty(item["Script Item"]))
            {
                this.ScriptItem = new ReferenceField(item.Fields["Script Item"]).TargetItem;
            }

            var parameters = item["Parameters"];
            this.Parameters = Sitecore.Web.WebUtil.ParseUrlParameters(parameters);
        }

        /// <summary>
        /// Runs the healthcheck.
        /// </summary>
        public override void RunHealthcheck()
        {
            this.LastCheckTime = DateTime.UtcNow;
            var invokeScript = $"ExecuteHealthcheck -componentId {this.InnerItem.ID.ToString()} -params {GetParamsFromNameValue(this.Parameters)}";
            try
            {
                using (ScriptSession scriptSession = ScriptSessionManager.NewSession("Default", true))
                {
                    string script = this.ScriptItem["Script"];
                    script += Environment.NewLine + invokeScript;
                    if (!string.IsNullOrEmpty(script))
                    {
                        var output = scriptSession.ExecuteScriptPart(script, false);

                        if (output.Count > 0)
                        {
                            Hashtable result = (Hashtable)output[0];
                            this.Status = (HealthcheckStatus)Enum.Parse(typeof(HealthcheckStatus), result["Status"].ToString());

                            if (this.Status != HealthcheckStatus.Healthy)
                            {
                                this.ErrorList.Entries.Add(new ErrorEntry
                                {
                                    Created = DateTime.UtcNow,
                                    Reason = result["Reason"].ToString()
                                });
                            }
                            else
                            {
                                this.HealthyMessage = result["HealthyMessage"].ToString();
                            }
                        }
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

        private string GetParamsFromNameValue(NameValueCollection collection)
        {
            string result = "@{";
            foreach (string key in collection.Keys)
            {
                result += $"{key} = \"{collection[key]}\";";
            }

            result += "}";

            return result;
        }
    }
}