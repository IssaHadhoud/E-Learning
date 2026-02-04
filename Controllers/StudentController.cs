using E_learning.Data;
using E_learning.Models;
using E_learning.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_learning.Controllers;

public class StudentController : Controller
{
    // private readonly UserViewModel ViewModel;
    private readonly ApplicationDbContext _context;

    private readonly IWebHostEnvironment _webHostEnvironment;

    public StudentController(ApplicationDbContext context,IWebHostEnvironment webHostEnvironment)
    {
        // ViewModel = viewModel;
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }
    public async Task <IActionResult> Index(Guid id){
        var student =_context.Course.FindAsync(id);
        return View(student);
    }


    public async Task< IActionResult> GetStudent(Guid id){
        var student =_context.Students.FindAsync(id);
        return View(student);
    }



}