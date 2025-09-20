using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpoBooking.Api.Data;
using ExpoBooking.Api.Models;

namespace ExpoBooking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SlotsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SlotsController> _logger;

        public SlotsController(AppDbContext db, ILogger<SlotsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // GET: api/slots
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SlotDto>>> Get()
        {
            var slots = await _db.Slots.OrderBy(s => s.StartTime).ToListAsync();
            var dtos = slots.Select(s => new SlotDto
            {
                Id = s.Id,
                StartTime = s.StartTime.ToString("yyyy-MM-dd HH:mm"),
                EndTime = s.EndTime.ToString("yyyy-MM-dd HH:mm"),
                Capacity = s.Capacity,
                BookedCount = s.BookedCount,
                IsAvailable = s.BookedCount < s.Capacity
            }).ToList();

            return Ok(dtos);
        }

        // POST: api/slots/{id}/book
        [HttpPost("{id}/book")]
        public async Task<IActionResult> Book(Guid id, [FromBody] BookingDto booking)
        {
            if (string.IsNullOrWhiteSpace(booking.Name) || string.IsNullOrWhiteSpace(booking.Email))
                return BadRequest(new { message = "Name and email are required." });

            // Load slot with concurrency token
            var slot = await _db.Slots.Where(s => s.Id == id).FirstOrDefaultAsync();
            if (slot == null) return NotFound(new { message = "Slot not found." });

            if (slot.BookedCount >= slot.Capacity)
            {
                return Conflict(new { message = "Slot is full." });
            }

            // Increase booked count, save with concurrency handling
            slot.BookedCount += 1;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict while booking slot {SlotId}", id);
                // Re-check availability
                var fresh = await _db.Slots.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
                if (fresh == null || fresh.BookedCount >= fresh.Capacity)
                    return Conflict(new { message = "Slot became full during booking attempt." });

                // Otherwise rethrow
                throw;
            }

            // Optionally, create a record of the booking in a new table; for brevity we return booking id as guid
            var bookingId = Guid.NewGuid();
            return Ok(new { bookingId });
        }
    }
}
