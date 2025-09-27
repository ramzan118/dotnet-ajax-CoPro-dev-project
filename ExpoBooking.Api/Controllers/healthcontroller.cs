using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ExpoBooking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HealthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetHealth()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            var healthInfo = new
            {
                Application = "Healthy",
                Timestamp = DateTime.UtcNow,
                Database = await CheckDatabaseConnection(connectionString)
            };

            return Ok(healthInfo);
        }

        private async Task<object> CheckDatabaseConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return new { Status = "Not Configured", Message = "Connection string is missing" };
            }

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                
                var serverVersion = await connection.QueryFirstOrDefaultAsync<string>("SELECT @@VERSION");
                var databaseName = await connection.QueryFirstOrDefaultAsync<string>("SELECT DB_NAME()");
                
                return new { 
                    Status = "Connected", 
                    Server = connection.DataSource,
                    Database = databaseName,
                    Version = serverVersion?.Substring(0, 50) + "..."
                };
            }
            catch (Exception ex)
            {
                return new { Status = "Disconnected", Error = ex.Message };
            }
        }
    }
}