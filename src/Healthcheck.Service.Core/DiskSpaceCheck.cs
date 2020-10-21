namespace Healthcheck.Service.Core
{
    using Healthcheck.Service.Customization;
    using Healthcheck.Service.Customization.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class DiskSpaceCheck
    {
        private const double BytesInGB = 1073741824;

        public static HealthcheckResult RunHealthcheck(string _driveName, double _errorPercentageThreshold, double _warningPercentageThreshold)
        {
            var checkResult = new HealthcheckResult
            {
                LastCheckTime = DateTime.UtcNow,
                Status = Customization.HealthcheckStatus.Healthy,
                HealthyMessage = "Log files contain no errors",
                ErrorList = new ErrorList
                {
                    Entries = new List<ErrorEntry>()
                }
            };

            if (string.IsNullOrEmpty(_driveName))
            {
                checkResult.Status = HealthcheckStatus.Warning;
                checkResult.ErrorList.Entries.Add(ErrorEntry.CreateErrorEntry("DriveName is not configured!"));

                return checkResult;
            }

            var drives = GetReadyDrives(checkResult);

            if (drives == null)
            {
                checkResult.Status = HealthcheckStatus.Error;
                checkResult.ErrorList.Entries.Add(ErrorEntry.CreateErrorEntry("Error retriving drives info."));

                return checkResult;
            }
            else if (drives.Count() == 0)
            {
                checkResult.Status = HealthcheckStatus.Error;
                checkResult.ErrorList.Entries.Add(ErrorEntry.CreateErrorEntry("Drives aren't ready."));

                return checkResult;
            }

            drives = drives.Where(t => t.DriveName.Equals(_driveName, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!drives.Any())
            {
                checkResult.Status = HealthcheckStatus.Warning;
                checkResult.ErrorList.Entries.Add(ErrorEntry.CreateErrorEntry($"Cannot find drive with name: {_driveName}"));

                return checkResult;
            }

            var errorDrivesCapacity = drives.Where(d => d.PercentageOfFreeSpace < _errorPercentageThreshold);
            if (errorDrivesCapacity.Count() > 0)
            {
                checkResult.Status = HealthcheckStatus.Error;
                checkResult.ErrorList.Entries.Add(ErrorEntry.CreateErrorEntry(CreateStatusMessage(errorDrivesCapacity, $"Drive: {_driveName} is on very low capacity.")));

                return checkResult;
            }

            var warningDrivesCapacity = drives.Where(d => d.PercentageOfFreeSpace < _warningPercentageThreshold);
            if (warningDrivesCapacity.Count() > 0)
            {
                checkResult.Status = HealthcheckStatus.Warning;
                checkResult.ErrorList.Entries.Add(ErrorEntry.CreateErrorEntry(CreateStatusMessage(warningDrivesCapacity, $"Drive: {_driveName} is on low capacity.")));

                return checkResult;
            }

            checkResult.Status = HealthcheckStatus.Healthy;
            checkResult.HealthyMessage = CreateStatusMessage(drives, $"Drive, {_driveName} are in good capacity.");

            return checkResult;
        }

        /// <summary>Creates the status message displayed to the user.</summary>
        /// <param name="drives">The drives.</param>
        /// <param name="message">The message.</param>
        /// <returns>The status message.</returns>
        private static string CreateStatusMessage(IEnumerable<DriveDetails> drives, string message)
        {
            var builder = new StringBuilder();
            builder.AppendLine(message);
            foreach (var drive in drives)
            {
                var driveBasic = $"Drive: {drive.DriveName}" +
                    $"\tFree space: {drive.AvailableFreeSpace:0.00} GB" +
                    $"\tPercentage free space: {drive.PercentageOfFreeSpace:0.00} %";

                builder.AppendLine(driveBasic);
            }

            return builder.ToString();
        }

        /// <summary>Gets the ready drives.</summary>
        /// <returns>A list of ready drives.</returns>
        private static List<DriveDetails> GetReadyDrives(HealthcheckResult checkResult)
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
                checkResult.ErrorList.Entries.Add(ErrorEntry.CreateErrorEntry(exception.Message, exception));
                return null;
            }

            return readyDrivesDetails;
        }
    }

    /// <summary>Store drive details.</summary>
    public class DriveDetails
    {
        /// <summary>Gets or sets the percentage of free space.</summary>
        /// <value>The percentage of free space.</value>
        public double PercentageOfFreeSpace { get; set; }

        /// <summary>Gets or sets the name of the drive.</summary>
        /// <value>The name of the drive.</value>
        public string DriveName { get; set; }

        /// <summary>Gets or sets the available free space.</summary>
        /// <value>The available free space.</value>
        public double AvailableFreeSpace { get; set; }
    }
}