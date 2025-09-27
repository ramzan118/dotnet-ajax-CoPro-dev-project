using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ExpoBooking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase  // Remove extra 'public' if any
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
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                
                if (string.IsNullOrEmpty(connectionString))
                {
                    return Ok(new { 
                        source = "Mock Data (No Connection String)", 
                        data = GetMockBookings(),
                        message = "Connection string is not configured" 
                    });
                }

                using var connection = new SqlConnection(connectionString);
                
                // Test connection
                await connection.OpenAsync();
                
                // Check if table exists
                var tableExists = await connection.QueryFirstOrDefaultAsync<int?>(
                    "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Bookings'");
                
                if (tableExists > 0)
                {
                    var bookings = await connection.QueryAsync("SELECT * FROM Bookings");
                    return Ok(new { source = "Database", data = bookings });
                }
                else
                {
                    return Ok(new { 
                        source = "Mock Data (Table Missing)", 
                        data = GetMockBookings(),
                        message = "Bookings table does not exist" 
                    });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { 
                    source = "Mock Data (Connection Failed)", 
                    data = GetMockBookings(),
                    error = ex.Message 
                });
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

        private List<object> GetMockBookings()
        {
            return new List<object>
            {
                new { Id = 1, Name = "Conference Booking", Email = "test1@example.com", EventDate = DateTime.Now.AddDays(30) },
                new { Id = 2, Name = "Workshop Booking", Email = "test2@example.com", EventDate = DateTime.Now.AddDays(45) }
            };
        }
    }

    public class BookingRequest
    {
        public string? Name { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string? EventType { get; set; } = string.Empty;
        public int Attendees { get; set; }
    }
}