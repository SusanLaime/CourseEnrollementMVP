using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CourseEnrollmentMVP.Data;
using CourseEnrollmentMVP.Data.Models;
using CourseEnrollmentMVP.Services;

namespace CourseEnrollmentMVP.Pages.Director;

[Authorize(Roles = "Director")]
public class DashboardModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IEmailSender _email;

    public DashboardModel(AppDbContext db, IEmailSender email)
    {
        _db = db;
        _email = email;
    }

    public class PendingView
    {
        public int EnrollmentId { get; set; }
        public string StudentName { get; set; } = "";
        public string CourseTitle { get; set; } = "";
        public DateTime AppliedAt { get; set; }
    }

    public List<PendingView> Pending { get; set; } = new();

    public void OnGet()
    {
        Pending = _db.Enrollments.Where(e => e.Status == "PENDING")
            .Join(_db.Users, e => e.StudentId, u => u.Id, (e, u) => new { e, u })
            .Join(_db.Courses, x => x.e.CourseId, c => c.Id, (x, c) => new PendingView
            {
                EnrollmentId = x.e.Id,
                StudentName = x.u.Name,
                CourseTitle = c.Title,
                AppliedAt = x.e.AppliedAt
            })
            .ToList();
    }

    public async Task<IActionResult> OnPostApprove(int enrollmentId)
    {
        var e = _db.Enrollments.Find(enrollmentId);
        if (e == null) return RedirectToPage();

        var course = _db.Courses.Find(e.CourseId);
        var student = _db.Users.Find(e.StudentId);
        if (course != null && student != null)
        {
            if (course.Enrolled >= course.Capacity)
            {
                e.Status = "DENIED";
                e.DecidedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                await _email.SendEmailAsync(student.Email, "Enrollment Denied - Full", $"Your enrollment for {course.Title} was denied because the course is full.");
                return RedirectToPage();
            }

            e.Status = "APPROVED";
            e.DecidedAt = DateTime.UtcNow;
            course.Enrolled += 1;
            await _db.SaveChangesAsync();

            await _email.SendEmailAsync(student.Email, "Enrollment Approved", $"Your enrollment for {course.Title} was approved.");
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeny(int enrollmentId)
    {
        var e = _db.Enrollments.Find(enrollmentId);
        if (e == null) return RedirectToPage();

        var course = _db.Courses.Find(e.CourseId);
        var student = _db.Users.Find(e.StudentId);

        e.Status = "DENIED";
        e.DecidedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        if (student != null)
            await _email.SendEmailAsync(student.Email, "Enrollment Denied", $"Your enrollment for {course?.Title ?? "a course"} was denied.");

        return RedirectToPage();
    }
}
