namespace Healthcheck.Service.Domain.LocalDisk
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Models;
    using Sitecore.Data.Items;

    /// <summary>Local disk space check.</summary>
    public class LocalDiskSpaceCheck : BaseComponent
    {
        /// <summary>The warning percentage threshold.</summary>
        private double _warningPercentageThreshold;

        /// <summary>The error percentage threshold.</summary>
        private double _errorPercentageThreshold;

        private const string WarningPercentageFieldName = "WarningPercentageThreshold";

        private const string ErrorPercentageFieldName = "ErrorPercentageThreshold";

        private const double WarningPercentageDefault = 25;

        private const double ErrorPercentageDefault = 10;

        private const double BytesInGB = 1073741824;

        public LocalDiskSpaceCheck(Item item) : base(item)
        {
            ReadParameters(item);
        }

        /// <summary>Runs the local disk space healthcheck.</summary>
        public override void RunHealthcheck()
        {
            this.LastCheckTime = DateTime.UtcNow;
            var drives = GetReadyDrives();

            if (drives == null)
            {
                this.Status = HealthcheckStatus.Error;
                this.ErrorList.Entries.Add(ErrorEntry.CreateErrorEntry("Error retriving drives info."));

                return;
            }
            else if (drives.Count() == 0)
            {
                this.Status = HealthcheckStatus.Error;
                this.ErrorList.Entries.Add(ErrorEntry.CreateErrorEntry("Drives aren't ready."));

                return;
            }

            var errorDrivesCapacity = drives.Where(d => d.PercentageOfFreeSpace < _errorPercentageThreshold);
            if (errorDrivesCapacity.Count() > 0)
            {
                this.Status = HealthcheckStatus.Error;
                this.ErrorList.Entries.Add(ErrorEntry.CreateErrorEntry(CreateStatusMessage(errorDrivesCapacity, "Some drives are very low capacity.")));

                return;
            }

            var warningDrivesCapacity = drives.Where(d => d.PercentageOfFreeSpace < _warningPercentageThreshold);
            if (warningDrivesCapacity.Count() > 0)
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(ErrorEntry.CreateErrorEntry(CreateStatusMessage(warningDrivesCapacity, "Some drives are low capacity.")));

                return;
            }
            
            this.Status = HealthcheckStatus.Healthy;
            this.HealthyMessage = CreateStatusMessage(drives, "Overall, drives are in good capacity.");
        }

        /// <summary>Creates the status message displayed to the user.</summary>
        /// <param name="drives">The drives.</param>
        /// <param name="message">The message.</param>
        /// <returns>The status message.</returns>
        private string CreateStatusMessage(IEnumerable<DriveDetails> drives, string message)
        {
            var builder = new StringBuilder();
            builder.AppendLine(message);
            foreach (var drive in drives)
            {
                var driveBasic = $"Drive: {drive.DriveName}" +
                    $"\tFree space: {drive.AvailableFreeSpace:0.00} GB" +
                    $"\tPercentagea free space: {drive.PercentageOfFreeSpace:0.00} %";

                builder.AppendLine(driveBasic);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Reads the parameters from the item.
        /// Store values on fields.
        /// </summary>
        /// <param name="item">The item.</param>
        private void ReadParameters(Item item)
        {
            var warningPercentageField = item.Fields[WarningPercentageFieldName];
            var errorPercentageField = item.Fields[ErrorPercentageFieldName];

            if (errorPercentageField != null)
            {
                var parsedErrorParameter = double.TryParse(errorPercentageField.Value, out _errorPercentageThreshold);
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
                var parsedWarningParameter = double.TryParse(warningPercentageField.Value, out _warningPercentageThreshold);
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

        /// <summary>Gets the ready drives.</summary>
        /// <returns>A list of ready drives.</returns>
        private List<DriveDetails> GetReadyDrives()
        {
            var readyDrivesDetails = new List<DriveDetails>();

            IEnumerable<DriveInfo> readyDrives;
            try
            {
                readyDrives = DriveInfo.GetDrives().Where(d => d.IsReady);

                foreach (var drive in readyDrives)
                {
                    var driveDetails = new DriveDetails();

                    driveDetails.DriveName = drive.Name;
                    driveDetails.AvailableFreeSpace = drive.AvailableFreeSpace / BytesInGB;
                    driveDetails.PercentageOfFreeSpace = (drive.AvailableFreeSpace / (double)drive.TotalSize) * 100;

                    readyDrivesDetails.Add(driveDetails);
                }
            }
            catch (Exception exception)
            {
                this.ErrorList.Entries.Add(ErrorEntry.CreateErrorEntry(exception.Message, exception));
                return null;
            }

            return readyDrivesDetails;
        }
    }
}