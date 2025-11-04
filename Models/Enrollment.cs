// Data/Models/Enrollment.cs
namespace CourseEnrollmentMVP.Data.Models;

public class Enrollment
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public string Status { get; set; } = "PENDING"; // PENDING, APPROVED, DENIED
    public DateTime AppliedAt { get; set; }
    public DateTime? DecidedAt { get; set; }
}