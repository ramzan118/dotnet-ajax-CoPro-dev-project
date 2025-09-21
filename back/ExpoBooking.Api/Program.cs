using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using ExpoBooking.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Update this line in your Program.cs
var keyVaultUrl = builder.Configuration["KeyVault:Url"] ?? "https://copro-vault-zffw1v3o.vault.azure.net/";

// Add Azure Key Vault configuration provider when KeyVault URL is present
if (!string.IsNullOrWhiteSpace(keyVaultUrl))
{
    try
    {
        var kvUri = new Uri(keyVaultUrl);
        builder.Configuration.AddAzureKeyVault(kvUri, new DefaultAzureCredential());
    }
    catch (Exception ex)
    {
        // If Key Vault cannot be added, log and continue using other configuration sources
        Console.WriteLine($"Warning: failed to add Azure Key Vault configuration provider: {ex.Message}");
    }
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Try to get a DB connection string from configuration (Key Vault, env, appsettings)
    var conn = builder.Configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrWhiteSpace(conn))
    {
        // If this is an Azure SQL connection string, use UseSqlServer; otherwise keep Sqlite fallback
        if (conn.IndexOf("Server=", StringComparison.OrdinalIgnoreCase) >= 0 ||
            conn.IndexOf(".database.windows.net", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            options.UseSqlServer(conn);
        }
        else
        {
            options.UseSqlite(conn);
        }
    }
    else
    {
        // Fallback to local SQLite
        options.UseSqlite("Data Source=expo.db");
    }
});

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
