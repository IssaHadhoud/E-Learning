using E_learning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_learning.Data;


public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Users>User { get; set; }
    public DbSet<Student>Students { get; set; }
    public DbSet<Teacher>Teachers { get; set; }
    public DbSet<Courses>Course { get; set; }
    
}
