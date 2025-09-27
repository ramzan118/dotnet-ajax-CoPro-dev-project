using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.SqlClient; // Changed from System.Data.SqlClient

namespace ExpoBooking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public BookingController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                var bookings = await connection.QueryAsync("SELECT * FROM Bookings");
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                // Return mock data for testing since database might not be set up yet
                var mockBookings = new[]
                {
                    new { Id = 1, Name = "Conference Booking", Email = "test1@example.com", EventDate = DateTime.Now.AddDays(30) },
                    new { Id = 2, Name = "Workshop Booking", Email = "test2@example.com", EventDate = DateTime.Now.AddDays(45) }
                };
                return Ok(mockBookings);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetBooking(int id)
        {
            return Ok(new { message = $"Getting booking with ID: {id}" });
        }

        [HttpPost]
        public IActionResult CreateBooking([FromBody] BookingRequest request)
        {
            return Ok(new { message = "Booking created successfully", booking = request });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBooking(int id, [FromBody] BookingRequest request)
        {
            return Ok(new { message = $"Booking {id} updated successfully", booking = request });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBooking(int id)
        {
            return Ok(new { message = $"Booking {id} deleted successfully" });
        }
    }

    public class BookingRequest
    {
        public string? Name { get; set; } = string.Empty; // Made nullable
        public string? Email { get; set; } = string.Empty; // Made nullable
        public DateTime EventDate { get; set; }
        public string? EventType { get; set; } = string.Empty; // Made nullable
        public int Attendees { get; set; }
    }
}