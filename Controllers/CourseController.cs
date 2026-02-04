using System.Security.Claims;
using E_learning.Data;
using E_learning.Models;
using E_learning.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_learning.Controllers;

public class CourseController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ILogger<ApplicationDbContext> _logger;

    public CourseController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment,
        ILogger<ApplicationDbContext> logger)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }

    // GET
    public async Task<IActionResult> Index()
    {
        return _context.Course != null
            ? View(await _context.Course.ToListAsync())
            : Problem("Entity set 'ApplicationDbContext.Course'  is null.");
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CourseViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var exsitedCours = await _context.Course.FirstOrDefaultAsync(c => c.Id == model.ID);
                if (exsitedCours != null)
                {
                    ModelState.AddModelError(string.Empty, "Course already exists.");
                    return View(model);
                }

                var course = new Courses
                {
                    Id = model.ID,
                    Details = model.Details,
                    Title = model.Title
                };

                course.ImageURL = model.Image != null ? await SaveFileAsync(model.Image, "Images") : null;
                course.VideoURL = model.Video != null ? await SaveFileAsync(model.Video, "Videos") : null;
                course.Documents = model.Document != null ? await SaveFileAsync(model.Document, "Document") : null;
                course.Assignment = model.Assignment != null
                    ? await SaveFileAsync(model.Assignment, "Assignment")
                    : null;
                _context.Course.Add(course);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index"); // Ensure this points to the correct action
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating course.");
                ModelState.AddModelError(string.Empty, "An error occurred. Please try again.");
            }
        }

        return View(model);
    }

    private async Task<string> SaveFileAsync(IFormFile file, string FolderName)
    {
        if (file == null)
        {
            return null;
        }

        var FolderPath = Path.Combine(_webHostEnvironment.WebRootPath, FolderName);
        Directory.CreateDirectory((FolderPath));
        var FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var FilePath = Path.Combine(FolderPath, FileName);
        using (var stream = new FileStream(FilePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Path.Combine(FolderName, FileName);
    }

    public async Task<IActionResult> Details(Guid? ID)
    {
        if (ID == null || _context.Course == null)
        {
            return NotFound();
        }

        var course = await _context.Course.FirstOrDefaultAsync(c => c.Id == ID);
        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    [HttpGet]
    public IActionResult Edit(Guid ID)
    {
        var course = _context.Course.Find(ID);
        if (course == null)
        {
            return NotFound();
        }

        var model = new CourseViewModel
        {
            ID = course.Id,
            Title = course.Title,
            Details = course.Details
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid ID, CourseViewModel model)
    {
        if (ModelState.IsValid)
        {
            var course = await _context.Course.FindAsync(ID);
            if (course == null)
            {
                return NotFound();
            }

            course.Title = model.Title;
            course.Details = model.Details;
            if (model.Image != null)
            {
                course.ImageURL = await SaveFileAsync(model.Image, "Images");
            }

            if (model.Video != null)
            {
                course.VideoURL = await SaveFileAsync(model.Video, "Videos");
            }

            if (model.Document != null)
            {
                course.Documents = await SaveFileAsync(model.Document, "Document");
            }

            // Save changes to the database
            _context.Course.Update(course);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index"); // Redirect to a suitable action or view
        }

        // If model state is invalid, return the view with the current model
        return View(model);
    }

    public async Task<IActionResult> Delete(Guid ID)
    {
        if (ID == null || _context.Course == null)
        {
            return NotFound();
        }

        var course = await _context.Course.FirstOrDefaultAsync(c => c.Id == ID);
        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid ID)
    {
        if (_context.Course == null)
        {
            return Problem("Entity set 'ApplicationDBContext.Course' is null.");
        }

        var course = await _context.Course.FindAsync(ID);
        if (course == null)
        {
            return NotFound();
        }

        _context.Course.Remove(course);

        // Define folders
        var folders = new Dictionary<string, string>
        {
            { course.ImageURL, "Images" },
            { course.VideoURL, "Videos" },
            { course.Documents, "Document" }
        };

        // Delete files
        foreach (var (fileUrl, folder) in folders)
        {
            if (!string.IsNullOrEmpty(fileUrl))
            {
                var fileName = Path.GetFileName(fileUrl);
                var filePath = Path.Combine(_webHostEnvironment.WebRootPath, folder, fileName);

                try
                {
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., logging the error)
                    Console.WriteLine($"Error deleting file '{fileName}': {ex.Message}");
                }
            }
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}