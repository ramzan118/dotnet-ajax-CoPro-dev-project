using ExpoBooking.Api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=expo.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocal", b => b.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:3000","http://localhost:5500","http://localhost:5001"));
});

var app = builder.Build();

app.UseCors("AllowLocal");
app.MapControllers();

// Seed on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    ExpoBooking.Api.Services.SeedData.Seed(db);
}

app.Run();
