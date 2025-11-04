// Data/Models/User.cs
namespace CourseEnrollmentMVP.Data.Models;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = "Student"; // Student or Director
    public string Name { get; set; } = "";
}