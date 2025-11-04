// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using CourseEnrollmentMVP.Data.Models;

namespace CourseEnrollmentMVP.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
}