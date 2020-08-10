namespace Healthcheck.Service.Domain.LocalDisk
{
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