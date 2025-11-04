// Data/Models/Course.cs
namespace CourseEnrollmentMVP.Data.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int Capacity { get; set; }
    public int Enrolled { get; set; }
}