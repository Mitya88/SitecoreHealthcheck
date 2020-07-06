using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Healthcheck.Service.Customization;
using Healthcheck.Service.Models;
using Sitecore.Data.Items;

namespace Healthcheck.Service.Domain.LocalDisk
{
    /// <summary>Local disk space check.</summary>
    public class LocalDiskSpaceCheck : BaseComponent
    {
        private double _warningPercentageThreshold;

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
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = "Error retriving drives info.",
                    Exception = null
                });

                return;
            }
            else if (drives.Count() == 0)
            {
                this.Status = HealthcheckStatus.Error;
                this.ErrorList.Entries.Add(ErrorEntry.GetDefaultErrorEntry("Drives aren't ready."));

                return;
            }

            var errorDrivesCapacity = drives.Where(d => d.PercentageOfFreeSpace < _errorPercentageThreshold);
            if (errorDrivesCapacity.Count() > 0)
            {
                this.Status = HealthcheckStatus.Error;
                this.ErrorList.Entries.Add(ErrorEntry.GetDefaultErrorEntry(CreateMessage(errorDrivesCapacity, "Some drives are very low capacity.")));

                return;
            }

            var warningDrivesCapacity = drives.Where(d => d.PercentageOfFreeSpace < _warningPercentageThreshold);
            if (warningDrivesCapacity.Count() > 0)
            {
                this.Status = HealthcheckStatus.Warning;
                this.ErrorList.Entries.Add(ErrorEntry.GetDefaultErrorEntry(CreateMessage(warningDrivesCapacity, "Some drives are low capacity.")));

                return;
            }
            
            this.Status = HealthcheckStatus.Healthy;
            this.HealthyMessage = CreateMessage(drives, "Overall, drives are in good capacity.");
        }

        private string CreateMessage(IEnumerable<DriveDetails> drives, string message)
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
                this.ErrorList.Entries.Add(new ErrorEntry
                {
                    Created = DateTime.UtcNow,
                    Reason = exception.Message,
                    Exception = exception
                });
                return null;
            }

            return readyDrivesDetails;
        }
    }
}