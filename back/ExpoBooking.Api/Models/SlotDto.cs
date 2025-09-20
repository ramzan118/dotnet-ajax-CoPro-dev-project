namespace ExpoBooking.Api.Models
{
    public class SlotDto
    {
        public Guid Id { get; set; }
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
        public int Capacity { get; set; }
        public int BookedCount { get; set; }
        public bool IsAvailable { get; set; }
    }
}
