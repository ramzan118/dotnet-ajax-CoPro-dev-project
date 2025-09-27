using Microsoft.AspNetCore.Mvc;

namespace ExpoBooking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = new[]
            {
                new { Id = 1, Name = "John Doe", Email = "john@example.com" },
                new { Id = 2, Name = "Jane Smith", Email = "jane@example.com" },
                new { Id = 3, Name = "Bob Johnson", Email = "bob@example.com" }
            };
            
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            return Ok(new { Id = id, Name = "Test User", Email = "test@example.com" });
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserRequest request)
        {
            return Ok(new { message = "User created successfully", user = request });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            return Ok(new { message = "Login successful", token = "sample-jwt-token" });
        }
    }

    public class UserRequest
    {
        public string? Name { get; set; } = string.Empty; // Made nullable
        public string? Email { get; set; } = string.Empty; // Made nullable
        public string? Password { get; set; } = string.Empty; // Made nullable
    }

    public class LoginRequest
    {
        public string? Email { get; set; } = string.Empty; // Made nullable
        public string? Password { get; set; } = string.Empty; // Made nullable
    }
}