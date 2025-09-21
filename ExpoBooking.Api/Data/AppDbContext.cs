using ExpoBooking.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpoBooking.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Slot> Slots { get; set; }
    }
}