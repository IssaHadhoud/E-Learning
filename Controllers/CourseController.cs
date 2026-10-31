using System.Security.Claims;
using System.Text.RegularExpressions;
using E_learning.Data;
using E_learning.Models;
using E_learning.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace E_learning.Controllers;

public class CourseController : Controller
{
    private const long MaxUploadBytes = 100 * 1024 * 1024;
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<CourseController> _logger;
    private readonly UserManager<ApplicationUser> _users;

    public CourseController(ApplicationDbContext db, IWebHostEnvironment environment, ILogger<CourseController> logger, UserManager<ApplicationUser> users)
    {
        _db = db; _environment = environment; _logger = logger; _users = users;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(string? search, int page = 1)
    {
        page = Math.Max(1, page);
        try
        {
            var query = _db.Course.AsNoTracking().Include(c => c.Category).Where(c => c.IsPublished);
            if (!string.IsNullOrWhiteSpace(search)) query = query.Where(c => c.Title.Contains(search) || c.Details.Contains(search));
            ViewData["Search"] = search;
            ViewData["Page"] = page;
            query = query.OrderByDescending(c => c.CreatedUtc);
            ViewData["HasMore"] = await query.Skip(page * 12).AnyAsync();
            return View(await query.Skip((page - 1) * 12).Take(12).ToListAsync());
        }
        catch (Exception ex) when (IsDatabaseConnectionFailure(ex))
        {
            _logger.LogError(ex, "Course catalog is unavailable because the database connection failed.");
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            ViewData["CatalogUnavailable"] = true;
            return View(Array.Empty<Courses>());
        }
    }

    [Authorize(Roles = "Teacher,Admin")]
    public IActionResult Create() => View(new CourseViewModel());

    [HttpPost, Authorize(Roles = "Teacher,Admin"), ValidateAntiForgeryToken, RequestSizeLimit(MaxUploadBytes)]
    public async Task<IActionResult> Create(CourseViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var course = new Courses
        {
            Id = Guid.NewGuid(), Title = model.Title.Trim(), Details = model.Details.Trim(),
            Slug = await CreateSlugAsync(model.Title), TeacherId = User.IsInRole("Admin") ? null : _users.GetUserId(User),
            IsPublished = false, CreatedUtc = DateTime.UtcNow, UpdatedUtc = DateTime.UtcNow
        };
        try
        {
            course.ImageURL = await SaveFileAsync(model.Image, "Images", [".jpg", ".jpeg", ".png", ".webp"]);
            course.VideoURL = await SaveFileAsync(model.Video, "Videos", [".mp4", ".webm"]);
            course.Documents = await SaveFileAsync(model.Document, "Document", [".pdf"]);
            course.Assignment = await SaveFileAsync(model.Assignment, "Assignment", [".pdf"]);
            _db.Course.Add(course);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidDataException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Course creation failed.");
            ModelState.AddModelError(string.Empty, "The course could not be saved. Please try again.");
            return View(model);
        }
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(Guid id)
    {
        Courses? course;
        try
        {
            course = await _db.Course.AsNoTracking().Include(c => c.Modules.OrderBy(m => m.SortOrder))
                .ThenInclude(m => m.Lessons.Where(l => l.IsPublished).OrderBy(l => l.SortOrder))
                .FirstOrDefaultAsync(c => c.Id == id && c.IsPublished);
        }
        catch (Exception ex) when (IsDatabaseConnectionFailure(ex))
        {
            _logger.LogError(ex, "Course detail is unavailable because the database connection failed.");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Course information is temporarily unavailable.");
        }
        if (course is null) return NotFound();
        if (User.IsInRole("Student"))
        {
            ViewData["Enrolled"] = await _db.Enrollments.AnyAsync(e => e.StudentId == _users.GetUserId(User) && e.CourseId == id);
            ViewData["FirstLessonId"] = course.Modules.OrderBy(m => m.SortOrder).SelectMany(m => m.Lessons).OrderBy(l => l.SortOrder).Select(l => (int?)l.Id).FirstOrDefault();
        }
        return View(course);
    }

    [Authorize(Roles = "Student")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(Guid id)
    {
        var studentId = _users.GetUserId(User)!;
        if (!await _db.Course.AnyAsync(c => c.Id == id && c.IsPublished)) return NotFound();
        if (await _db.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == id))
        {
            TempData["Notice"] = "You are already enrolled in this course.";
            return RedirectToAction(nameof(Details), new { id });
        }
        _db.Enrollments.Add(new Enrollment { StudentId = studentId, CourseId = id, EnrolledUtc = DateTime.UtcNow });
        try { await _db.SaveChangesAsync(); }
        catch (DbUpdateException)
        {
            TempData["Notice"] = "You are already enrolled in this course.";
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Learn(Guid courseId, int lessonId)
    {
        var studentId = _users.GetUserId(User)!;
        var enrollment = await _db.Enrollments.AsNoTracking().Include(e => e.Course).ThenInclude(c => c.Modules)
            .ThenInclude(m => m.Lessons.Where(l => l.IsPublished))
            .Include(e => e.LessonProgress)
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId && e.Course.IsPublished);
        if (enrollment is null) return NotFound();
        var lessons = enrollment.Course.Modules.OrderBy(m => m.SortOrder).SelectMany(m => m.Lessons.OrderBy(l => l.SortOrder)).ToList();
        var index = lessons.FindIndex(l => l.Id == lessonId);
        if (index < 0) return NotFound();
        var completedIds = enrollment.LessonProgress.Where(p => p.CompletedUtc.HasValue).Select(p => p.LessonId).ToHashSet();
        return View(new LessonLearningViewModel
        {
            Course = enrollment.Course, Lesson = lessons[index],
            PreviousLessonId = index > 0 ? lessons[index - 1].Id : null,
            NextLessonId = index + 1 < lessons.Count ? lessons[index + 1].Id : null,
            IsCompleted = completedIds.Contains(lessonId),
            ProgressPercent = lessons.Count == 0 ? 0 : (int)Math.Round(completedIds.Count * 100d / lessons.Count)
        });
    }

    [Authorize(Roles = "Student"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkLessonComplete(Guid courseId, int lessonId)
    {
        var studentId = _users.GetUserId(User)!;
        var enrollment = await _db.Enrollments.Include(e => e.Course).ThenInclude(c => c.Modules).ThenInclude(m => m.Lessons)
            .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId && e.Course.IsPublished);
        if (enrollment is null || !enrollment.Course.Modules.SelectMany(m => m.Lessons).Any(l => l.Id == lessonId && l.IsPublished)) return NotFound();
        var progress = await _db.LessonProgress.FirstOrDefaultAsync(p => p.EnrollmentId == enrollment.Id && p.LessonId == lessonId);
        var alreadyCompleted = progress?.CompletedUtc.HasValue == true;
        if (progress is null)
        {
            progress = new LessonProgress { EnrollmentId = enrollment.Id, LessonId = lessonId };
            _db.LessonProgress.Add(progress);
        }
        progress.CompletedUtc ??= DateTime.UtcNow;
        progress.LastAccessedUtc = DateTime.UtcNow;
        var publishedLessonIds = enrollment.Course.Modules.SelectMany(m => m.Lessons).Where(l => l.IsPublished).Select(l => l.Id).ToHashSet();
        var completedCount = await _db.LessonProgress.CountAsync(p => p.EnrollmentId == enrollment.Id && p.CompletedUtc != null && publishedLessonIds.Contains(p.LessonId));
        if (publishedLessonIds.Count > 0 && completedCount + (alreadyCompleted ? 0 : 1) >= publishedLessonIds.Count)
            enrollment.CompletedUtc ??= DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Learn), new { courseId, lessonId });
    }

    [Authorize(Roles = "Teacher,Admin")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var course = await GetManagedCourseAsync(id);
        if (course is null) return NotFound();
        return View(new CourseViewModel { ID = course.Id, Title = course.Title, Details = course.Details });
    }

    [HttpPost, Authorize(Roles = "Teacher,Admin"), ValidateAntiForgeryToken, RequestSizeLimit(MaxUploadBytes)]
    public async Task<IActionResult> Edit(Guid id, CourseViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var course = await GetManagedCourseAsync(id);
        if (course is null) return NotFound();
        course.Title = model.Title.Trim(); course.Details = model.Details.Trim(); course.UpdatedUtc = DateTime.UtcNow;
        try
        {
            course.ImageURL = await SaveOrKeepAsync(model.Image, "Images", [".jpg", ".jpeg", ".png", ".webp"], course.ImageURL);
            course.VideoURL = await SaveOrKeepAsync(model.Video, "Videos", [".mp4", ".webm"], course.VideoURL);
            course.Documents = await SaveOrKeepAsync(model.Document, "Document", [".pdf"], course.Documents);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (InvalidDataException ex) { ModelState.AddModelError(string.Empty, ex.Message); return View(model); }
    }

    private Task<Courses?> GetManagedCourseAsync(Guid id) => _db.Course.FirstOrDefaultAsync(c => c.Id == id && (User.IsInRole("Admin") || c.TeacherId == _users.GetUserId(User)));

    private static bool IsDatabaseConnectionFailure(Exception exception)
        => exception is SqlException || exception.InnerException is SqlException || exception.InnerException?.InnerException is SqlException;

    private async Task<string> CreateSlugAsync(string title)
    {
        var baseSlug = Regex.Replace(title.ToLowerInvariant(), "[^a-z0-9]+", "-").Trim('-');
        if (baseSlug.Length > 150) baseSlug = baseSlug[..150].Trim('-');
        var slug = string.IsNullOrEmpty(baseSlug) ? Guid.NewGuid().ToString("N") : baseSlug;
        while (await _db.Course.AnyAsync(c => c.Slug == slug)) slug = $"{baseSlug}-{Guid.NewGuid():N}";
        return slug;
    }

    private async Task<string?> SaveOrKeepAsync(IFormFile? file, string folder, string[] extensions, string? current)
        => file is null || file.Length == 0 ? current : await SaveFileAsync(file, folder, extensions);

    private async Task<string?> SaveFileAsync(IFormFile? file, string folder, string[] extensions)
    {
        if (file is null || file.Length == 0) return null;
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (file.Length > MaxUploadBytes || !extensions.Contains(extension))
            throw new InvalidDataException($"Upload must be under 100 MB and use: {string.Join(", ", extensions)}.");
        var directory = Path.Combine(_environment.WebRootPath, folder);
        Directory.CreateDirectory(directory);
        var name = $"{Guid.NewGuid():N}{extension}";
        await using var stream = System.IO.File.Create(Path.Combine(directory, name));
        await file.CopyToAsync(stream);
        return $"{folder}/{name}";
    }
}
