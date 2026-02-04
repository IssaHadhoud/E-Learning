using E_learning.Data;
using E_learning.Models;
using E_learning.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace E_learning.Controllers;

public class UserController : Controller
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ApplicationDbContext _context;


    public UserController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
        _context = context;
    }

    // GET
    public IActionResult Index(Guid ID)
    {
        var user = _context.Users.Find(ID);
        return View(user);
    }

    public IActionResult Register()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(U => U.Email == model.Email);
            if (existingUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email is already existed");
                return View(model);
            }

            var user = new Users()
            {
                Email = model.Email,
                Password = model.PassWord,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Department = model.Department
            };
            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Course");
        }

        return View(model);
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _context.User.FirstOrDefaultAsync(i =>
                i.Email == model.Email && i.Password == model.Password);
            if (user != null)
            {
                return RedirectToAction("Index", "Course");
            }
        }

        ModelState.AddModelError(string.Empty, "Invalid Login");
        return View(model);
    }
}