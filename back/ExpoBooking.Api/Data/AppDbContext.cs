using Microsoft.EntityFrameworkCore;
using ExpoBooking.Api.Models;

namespace ExpoBooking.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Slot> Slots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Slot>()
                .Property(s => s.RowVersion)
                .IsRowVersion();
        }
    }
}
