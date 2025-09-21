using System.ComponentModel.DataAnnotations;

namespace ExpoBooking.Api.Models
{
    public class Slot
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}