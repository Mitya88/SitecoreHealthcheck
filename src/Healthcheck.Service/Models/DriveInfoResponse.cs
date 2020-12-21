namespace Healthcheck.Service.Models
{
    public class DriveInfoResponse
    {
        public string DriveLetter { get; set; }

        public long FreeCapacity { get; set; }

        public long UsedCapacity { get; set; }

        public string FreeCapacityText { get; set; }

        public string UsedCapacityText { get; set; }
    }
}