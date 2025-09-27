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
        
        // Check if table exists and get real data
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

private List<object> GetMockBookings()
{
    return new List<object>
    {
        new { Id = 1, Name = "Conference Booking", Email = "test1@example.com", EventDate = DateTime.Now.AddDays(30) },
        new { Id = 2, Name = "Workshop Booking", Email = "test2@example.com", EventDate = DateTime.Now.AddDays(45) }
    };
}