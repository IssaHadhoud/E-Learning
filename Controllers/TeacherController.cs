using E_learning.Data;
using Microsoft.AspNetCore.Mvc;

namespace E_learning.Controllers;

public class TeacherController : Controller
{
    private readonly ApplicationDbContext _context;

    public TeacherController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET
    public async Task<IActionResult> Index(Guid id)
    {
        var teaher = await _context.Teachers.FindAsync(id);
        return View(teaher);
    }

    public IActionResult add()
    {
        
        
        return View();
    }
    
    
    
}