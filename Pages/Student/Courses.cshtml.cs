// Pages/Student/Courses.cshtml.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CourseEnrollmentMVP.Data;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace CourseEnrollmentMVP.Pages.Student;

[Authorize(Roles = "Student")]
public class CoursesModel : PageModel
{
    private readonly AppDbContext _db;
    public List<Data.Models.Course> Courses { get; set; } = new();

    public CoursesModel(AppDbContext db) => _db = db;

    public void OnGet()
    {
        Courses = _db.Courses.ToList();
    }

    public async Task<IActionResult> OnPostRegisterAsync(int courseId)
    {
        var userId = int.Parse(User.FindFirst("UserId")!.Value);
        var enrollment = new Data.Models.Enrollment
        {
            StudentId = userId,
            CourseId = courseId,
            Status = "PENDING",
            AppliedAt = DateTime.UtcNow
        };
        _db.Enrollments.Add(enrollment);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Registration submitted!";
        return RedirectToPage();
    }
}