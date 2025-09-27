using Microsoft.AspNetCore.Mvc;

namespace ExpoBooking.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "ExpoBooking API is running!", timestamp = DateTime.UtcNow });
        }
    }
}