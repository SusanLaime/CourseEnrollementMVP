// Program.cs (replace all)
using Microsoft.EntityFrameworkCore;
using CourseEnrollmentMVP.Data;
using CourseEnrollmentMVP.Data.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "jwt";
        config.LoginPath = "/Login";
    });

var app = builder.Build();

// Seed DB
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // If this project has EF migrations checked in, apply them.
    // If no migrations exist (development/new repo), fall back to EnsureCreated so the
    // SQLite schema is created for local development.
    var availableMigrations = db.Database.GetMigrations();
    if (availableMigrations != null && availableMigrations.Any())
    {
        db.Database.Migrate();
    }
    else
    {
        db.Database.EnsureCreated();
    }

    // EnsureCreated again as a safety-net for developer runs where migrations
    // might not create the schema as expected. This makes the app tolerant to
    // missing migrations in local/dev environments. Long-term, add migrations
    // and remove EnsureCreated.
    db.Database.EnsureCreated();

    if (!db.Users.Any())
    {
        var student = new User
        {
            Email = "student@edu.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass123"),
            Role = "Student",
            Name = "Alice"
        };
        var director = new User
        {
            Email = "director@edu.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass123"),
            Role = "Director",
            Name = "Bob"
        };
        db.Users.AddRange(student, director);

        db.Courses.AddRange(
            new Course { Title = "AI 101", Description = "Intro to AI", Capacity = 30, Enrolled = 0, MajorsAllowed = "ISC,ECO", Semester = "Fall 2025" },
            new Course { Title = "Web Dev", Description = "HTML/CSS/JS", Capacity = 25, Enrolled = 0, MajorsAllowed = "ISC", Semester = "Spring 2026" }
        );

        db.SaveChanges();
    }
}

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapGet("/", () => Results.Redirect("/Login"));

app.Run();