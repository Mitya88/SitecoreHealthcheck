namespace Healthcheck.Service.Domain
{
    using Sitecore.Data.Items;

    /// <summary>Local disk space check.</summary>
    public class LocalDiskSpaceCheck : BaseComponent
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

        public LocalDiskSpaceCheck(Item item) : base(item)
        {
            ReadParameters(item);
        }

        /// <summary>Runs the local disk space healthcheck.</summary>
        public override void RunHealthcheck()
        {
            var result = Healthcheck.Service.Core.DiskSpaceCheck.RunHealthcheck(this._driveName, this._errorPercentageThreshold, this._warningPercentageThreshold);

            this.Status = result.Status;
            this.HealthyMessage = result.HealthyMessage;
            this.ErrorList = result.ErrorList;
            this.LastCheckTime = result.LastCheckTime;
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