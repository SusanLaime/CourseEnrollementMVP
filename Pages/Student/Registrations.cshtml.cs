using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CourseEnrollmentMVP.Data;
using CourseEnrollmentMVP.Data.Models;
using System.Linq;
using System.Collections.Generic;

namespace CourseEnrollmentMVP.Pages.Student;

[Authorize(Roles = "Student")]
public class RegistrationsModel : PageModel
{
    private readonly AppDbContext _db;

    public RegistrationsModel(AppDbContext db) => _db = db;

    public class GroupView
    {
        public string CourseTitle { get; set; } = "";
        public List<Enrollment> Enrollments { get; set; } = new();
    }

    public List<GroupView> Grouped { get; set; } = new();

    public void OnGet()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        if (!int.TryParse(userIdClaim, out var uid)) return;

        var items = _db.Enrollments
            .Where(e => e.StudentId == uid)
            .Join(_db.Courses, e => e.CourseId, c => c.Id, (e, c) => new { e, c.Title })
            .ToList()
            .GroupBy(x => x.Title)
            .Select(g => new GroupView
            {
                CourseTitle = g.Key,
                Enrollments = g.Select(x => x.e).OrderByDescending(en => en.AppliedAt).ToList()
            })
            .ToList();

        Grouped = items;
    }
}
