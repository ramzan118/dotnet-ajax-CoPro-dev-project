using ExpoBooking.Api.Data;
using ExpoBooking.Api.Models;

namespace ExpoBooking.Api.Services
{
    public static class SeedData
    {
        public static void Seed(AppDbContext db)
        {
            if (db.Slots.Any()) return;

            db.Slots.AddRange(new[]
            {
                new Slot { Id = Guid.NewGuid(), StartTime = DateTime.Today.AddHours(9), EndTime = DateTime.Today.AddHours(10), Capacity = 50, BookedCount = 0 },
                new Slot { Id = Guid.NewGuid(), StartTime = DateTime.Today.AddHours(10), EndTime = DateTime.Today.AddHours(11), Capacity = 50, BookedCount = 0 },
                new Slot { Id = Guid.NewGuid(), StartTime = DateTime.Today.AddHours(11), EndTime = DateTime.Today.AddHours(12), Capacity = 30, BookedCount = 0 }
            });

            db.SaveChanges();
        }
    }
}
