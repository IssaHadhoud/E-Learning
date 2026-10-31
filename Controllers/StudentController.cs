using System.Security.Claims;
using E_learning.Data;
using E_learning.Models;
using E_learning.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_learning.Controllers;

[Authorize(Roles = "Student")]
public class StudentController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    public StudentController(ApplicationDbContext db, UserManager<ApplicationUser> users) { _db = db; _users = users; }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _users.FindByIdAsync(userId);
        var enrollments = await _db.Enrollments.AsNoTracking().Where(e => e.StudentId == userId)
            .Include(e => e.Course).ThenInclude(c => c.Modules).ThenInclude(m => m.Lessons)
            .Include(e => e.LessonProgress).ToListAsync();
        var model = new StudentDashboardViewModel
        {
            FirstName = user?.FirstName ?? "learner",
            Courses = enrollments.Select(e =>
            {
                var lessons = e.Course.Modules.SelectMany(m => m.Lessons).Where(l => l.IsPublished).ToList();
                var completed = e.LessonProgress.Count(p => p.CompletedUtc.HasValue && lessons.Any(l => l.Id == p.LessonId));
                return new EnrolledCourseViewModel { CourseId = e.CourseId, Title = e.Course.Title, ImageUrl = e.Course.ImageURL, TotalLessons = lessons.Count, CompletedLessons = completed, FirstLessonId = e.Course.Modules.OrderBy(m => m.SortOrder).SelectMany(m => m.Lessons).Where(l => l.IsPublished).OrderBy(l => l.SortOrder).Select(l => (int?)l.Id).FirstOrDefault() };
            }).ToList()
        };
        return View(model);
    }
}
