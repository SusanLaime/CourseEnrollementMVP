// Pages/Director/Dashboard.cshtml.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CourseEnrollmentMVP.Data;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace CourseEnrollmentMVP.Pages.Director;

[Authorize(Roles = "Director")]
public class DashboardModel : PageModel
{
    private readonly AppDbContext _db;
    public List<dynamic> Pending { get; set; } = new();

    public DashboardModel(AppDbContext db) => _db = db;

    public void OnGet()
    {
        Pending = _db.Enrollments
            .Where(e => e.Status == "PENDING")
            .Join(_db.Users, e => e.StudentId, u => u.Id, (e, u) => new { e, u })
            .Join(_db.Courses, x => x.e.CourseId, c => c.Id, (x, c) => new
            {
                x.e.Id,
                StudentName = x.u.Name,
                CourseTitle = c.Title,
                AppliedAt = x.e.AppliedAt
            })
            .ToList<dynamic>();
    }

    public async Task<IActionResult> OnPostApproveAsync(int id)
    {
        var e = _db.Enrollments.Find(id);
        if (e != null)
        {
            e.Status = "APPROVED";
            e.DecidedAt = DateTime.UtcNow;
            var course = _db.Courses.Find(e.CourseId);
            if (course != null) course.Enrolled++;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDenyAsync(int id)
    {
        var e = _db.Enrollments.Find(id);
        if (e != null)
        {
            e.Status = "DENIED";
            e.DecidedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        return RedirectToPage();
    }
}