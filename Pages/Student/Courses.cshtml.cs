using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CourseEnrollmentMVP.Data;
using CourseEnrollmentMVP.Data.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CourseEnrollmentMVP.Pages.Student;

[Authorize(Roles = "Student")]
public class CoursesModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly CourseEnrollmentMVP.Services.IEmailSender _email;

    public CoursesModel(AppDbContext db, CourseEnrollmentMVP.Services.IEmailSender email)
    {
        _db = db;
        _email = email;
    }

    public List<Course> Courses { get; set; } = new();
    public List<string> Majors { get; set; } = new();
    public List<string> Semesters { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string SelectedMajor { get; set; } = "";

    [BindProperty(SupportsGet = true)]
    public string SelectedSemester { get; set; } = "";

    public void OnGet()
    {
        // Populate majors from courses (split MajorsAllowed)
        Majors = _db.Courses
            .AsEnumerable()
            .SelectMany(c => (c.MajorsAllowed ?? "").Split(',', System.StringSplitOptions.RemoveEmptyEntries))
            .Select(m => m.Trim())
            .Distinct()
            .OrderBy(m => m)
            .ToList();

        Semesters = _db.Courses
            .Select(c => c.Semester ?? "")
            .Where(s => !string.IsNullOrEmpty(s))
            .Distinct()
            .OrderBy(s => s)
            .ToList();

        var query = _db.Courses.AsQueryable();
        if (!string.IsNullOrEmpty(SelectedMajor))
        {
            query = query.Where(c => (c.MajorsAllowed ?? "").Contains(SelectedMajor));
        }
        if (!string.IsNullOrEmpty(SelectedSemester))
        {
            query = query.Where(c => c.Semester == SelectedSemester);
        }

        Courses = query.ToList();
    }

    public async Task<IActionResult> OnPostRegisterAsync(int courseId)
    {
        var uidClaim = User.FindFirst("UserId")?.Value;
        if (!int.TryParse(uidClaim, out var uid)) return RedirectToPage("/Login");

        var course = _db.Courses.Find(courseId);
        if (course == null) return RedirectToPage();

        // Prevent duplicate registration
        if (_db.Enrollments.Any(e => e.CourseId == courseId && e.StudentId == uid))
        {
            TempData["Success"] = "You have already applied.";
            return RedirectToPage();
        }

        var enrollment = new Enrollment
        {
            StudentId = uid,
            CourseId = courseId,
            Status = "PENDING",
            AppliedAt = DateTime.UtcNow
        };
        _db.Enrollments.Add(enrollment);
        await _db.SaveChangesAsync();

        // Send pre-registration notification emails
        var student = _db.Users.Find(uid);
        var directors = _db.Users.Where(u => u.Role == "Director").ToList();

        if (student != null)
        {
            var studentBody = $"Dear {student.Name},\n\nYour registration for '{course.Title}' has been received and is pending approval.\nCourse: {course.Title}\nSemester: {course.Semester}\n\nRegards\nCourse Enrollment System";
            await _email.SendEmailAsync(student.Email, "Pre-registration received", studentBody);
        }

        foreach (var dir in directors)
        {
            var directorBody = $"Hello {dir.Name},\n\nStudent {student?.Name ?? "(unknown)"} has submitted a registration for '{course.Title}'.\nPlease review and approve or deny the request in the Director Dashboard.\n\nRegards\nCourse Enrollment System";
            await _email.SendEmailAsync(dir.Email, "New pre-registration pending", directorBody);
        }

        TempData["Success"] = "Registration submitted!";
        // After registration, show the student's registrations grouped by subject
        return RedirectToPage("/Student/Registrations");
    }
}
