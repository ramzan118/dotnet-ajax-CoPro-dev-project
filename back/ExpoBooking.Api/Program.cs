using ExpoBooking.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=expo.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocal", b => b.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000","http://localhost:5500","http://localhost:5001"));
});

// Add logging
builder.Logging.AddConsole();

var app = builder.Build();

app.UseCors("AllowLocal");
app.MapControllers();

// Seed on startup with better error handling
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        // Ensure database is created
        db.Database.EnsureCreated();
        
        // Apply migrations if any
        if (db.Database.GetPendingMigrations().Any())
        {
            logger.LogInformation("Applying pending migrations...");
            db.Database.Migrate();
            logger.LogInformation("Migrations applied successfully.");
        }
        else
        {
            logger.LogInformation("No pending migrations.");
        }
        
        // Seed data
        ExpoBooking.Api.Services.SeedData.Seed(db);
        logger.LogInformation("Database seeded successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

app.Run();