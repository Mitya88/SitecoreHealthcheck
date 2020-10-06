namespace Healthcheck.Service.Domain.Remote
{
    using Healthcheck.Service.Core;
    using Healthcheck.Service.Core.Messages;
    using Microsoft.Azure.ServiceBus.Core;
    using Newtonsoft.Json;
    using Sitecore.Data.Items;
    using System;
    using System.Text;

    /// <summary>Remote disk space check.</summary>
    public class RemoteDiskSpaceCheck : RemoteBaseComponent
    {
        /// <summary>The warning percentage threshold.</summary>
        private double _warningPercentageThreshold;

        /// <summary>The error percentage threshold.</summary>
        private double _errorPercentageThreshold;

        private string _driveName;

        private const string WarningPercentageFieldName = "WarningPercentageThreshold";

        private const string ErrorPercentageFieldName = "ErrorPercentageThreshold";

        private const string DriveNameFieldName = "DriveName";

        private const double WarningPercentageDefault = 25;

        private const double ErrorPercentageDefault = 10;

        /// <summary>Initializes a new instance of the <a onclick="return false;" href="RemoteDiskSpaceCheck" originaltag="see">RemoteDiskSpaceCheck</a> class.</summary>
        /// <param name="item">The item.</param>
        public RemoteDiskSpaceCheck(Item item) : base(item)
        {
            ReadParameters(item);
        }

        /// <summary>Runs the local disk space healthcheck.</summary>
        public override void RunHealthcheck()
        {
            var dateTime = DateTime.UtcNow;
            this.SaveRemoteCheckStarted(dateTime);

            var messageSender = new MessageSender(SharedConfig.ConnectionStringOrKey, SharedConfig.TopicName);

            var message = new OutGoingMessage
            {
                Parameters = new System.Collections.Generic.Dictionary<string, string>
                {
                    {"DriveName", this._driveName },
                    {"ErrorPercentageThreshold", this._errorPercentageThreshold.ToString() },
                    {"WarningPercentageThreshold", this._warningPercentageThreshold.ToString() }
                },
                TargetInstance = this.TargetInstance,
                ComponentId = this.InnerItem.ID.Guid,
                EventRaised = dateTime
            };

            var busMessage = new Microsoft.Azure.ServiceBus.Message
            {
                ContentType = "application/json",
                Label = Constants.TemplateNames.RemoteDiskSpaceCheckTemplateName,
                Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message))
            };

            messageSender.SendAsync(busMessage).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Reads the parameters from the item.
        /// Store values on fields.
        /// </summary>
        /// <param name="item">The item.</param>
        private void ReadParameters(Item item)
        {
            var warningPercentageField = item[WarningPercentageFieldName];
            var errorPercentageField = item[ErrorPercentageFieldName];
            _driveName = item[DriveNameFieldName];

            if (errorPercentageField != null)
            {
                var parsedErrorParameter = double.TryParse(errorPercentageField, out _errorPercentageThreshold);
                if (!parsedErrorParameter)
                {
                    _errorPercentageThreshold = ErrorPercentageDefault;
                }
            }
            else
            {
                _errorPercentageThreshold = ErrorPercentageDefault;
            }

            if (warningPercentageField != null)
            {
                var parsedWarningParameter = double.TryParse(warningPercentageField, out _warningPercentageThreshold);
                if (!parsedWarningParameter)
                {
                    _warningPercentageThreshold = WarningPercentageDefault;
                }
            }
            else
            {
                _warningPercentageThreshold = WarningPercentageDefault;
            }
        }
    }
}