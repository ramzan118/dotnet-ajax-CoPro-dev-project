using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpoBooking.Api.Models
{
    public class Slot
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int Capacity { get; set; }

        // Count of already booked seats
        public int BookedCount { get; set; }

        // Concurrency token
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
