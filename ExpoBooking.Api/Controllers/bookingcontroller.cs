[HttpGet]
public async Task<IActionResult> GetBookings()
{
    try
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            return Ok(new
            {
                source = "Mock Data (No Connection String)",
                data = GetMockBookings(),
                message = "Connection string is not configured"
            });
        }

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var tableExists = await connection.QueryFirstOrDefaultAsync<int?>(
            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Bookings'");

        if (tableExists > 0)
        {
            var bookings = await connection.QueryAsync("SELECT * FROM Bookings");
            return Ok(new { source = "Database", data = bookings });
        }

        return Ok(new
        {
            source = "Mock Data (Table Missing)",
            data = GetMockBookings(),
            message = "Bookings table does not exist"
        });
    }
    catch (Exception ex)
    {
        return Ok(new
        {
            source = "Mock Data (Connection Failed)",
            data = GetMockBookings(),
            error = ex.Message
        });
    }
}
