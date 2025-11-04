// Data/Models/Course.cs
namespace CourseEnrollmentMVP.Data.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int Capacity { get; set; }
    public int Enrolled { get; set; }
    // Comma-separated allowed majors, e.g. "ISC,ECO"
    public string MajorsAllowed { get; set; } = "ISC,ECO";
    // Semester label, e.g. "Fall 2025"
    public string Semester { get; set; } = "";
}