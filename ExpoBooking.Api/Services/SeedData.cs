using ExpoBooking.Api.Data;
using ExpoBooking.Api.Models;

namespace ExpoBooking.Api.Services
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Slots.Any())
            {
                return;
            }

            var slots = new Slot[]
            {
                new Slot { StartTime = DateTime.Today.AddHours(9),  EndTime = DateTime.Today.AddHours(10), IsAvailable = true },
                new Slot { StartTime = DateTime.Today.AddHours(10), EndTime = DateTime.Today.AddHours(11), IsAvailable = true },
                new Slot { StartTime = DateTime.Today.AddHours(11), EndTime = DateTime.Today.AddHours(12), IsAvailable = true },
                new Slot { StartTime = DateTime.Today.AddHours(13), EndTime = DateTime.Today.AddHours(14), IsAvailable = true },
                new Slot { StartTime = DateTime.Today.AddHours(14), EndTime = DateTime.Today.AddHours(15), IsAvailable = true },
                new Slot { StartTime = DateTime.Today.AddHours(15), EndTime = DateTime.Today.AddHours(16), IsAvailable = true },
                new Slot { StartTime = DateTime.Today.AddHours(16), EndTime = DateTime.Today.AddHours(17), IsAvailable = true }
            };

            context.Slots.AddRange(slots);
            context.SaveChanges();
        }
    }
}