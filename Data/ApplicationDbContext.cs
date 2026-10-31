using E_learning.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_learning.Data;


public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Users>User { get; set; }
    public DbSet<Student>Students { get; set; }
    public DbSet<Teacher>Teachers { get; set; }
    public DbSet<Courses>Course { get; set; }
    public DbSet<CourseCategory> CourseCategories => Set<CourseCategory>();
    public DbSet<CourseModule> CourseModules => Set<CourseModule>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<LessonProgress> LessonProgress => Set<LessonProgress>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>().Property(x => x.CreatedUtc).HasDefaultValueSql("SYSUTCDATETIME()");
        builder.Entity<Courses>(entity =>
        {
            entity.Property(x => x.Title).IsRequired();
            entity.Property(x => x.Details).HasMaxLength(5000).IsRequired();
            entity.Property(x => x.Slug).HasMaxLength(180);
            entity.HasIndex(x => x.Slug).IsUnique().HasFilter("[Slug] IS NOT NULL");
            entity.Property(x => x.Price).HasPrecision(10, 2);
            entity.Property(x => x.CreatedUtc).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.Property(x => x.UpdatedUtc).HasDefaultValueSql("SYSUTCDATETIME()");
            entity.HasOne(x => x.Category).WithMany(x => x.Courses).HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(x => x.Teacher).WithMany().HasForeignKey(x => x.TeacherId).OnDelete(DeleteBehavior.SetNull);
        });
        builder.Entity<CourseCategory>().HasIndex(x => x.Slug).IsUnique();
        builder.Entity<CourseModule>().HasOne(x => x.Course).WithMany(x => x.Modules).HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Lesson>().HasOne(x => x.CourseModule).WithMany(x => x.Lessons).HasForeignKey(x => x.CourseModuleId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<Enrollment>().HasIndex(x => new { x.StudentId, x.CourseId }).IsUnique();
        builder.Entity<Enrollment>().HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Enrollment>().HasOne(x => x.Course).WithMany(x => x.Enrollments).HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.Restrict);
        builder.Entity<LessonProgress>().HasIndex(x => new { x.EnrollmentId, x.LessonId }).IsUnique();
        builder.Entity<LessonProgress>().HasOne(x => x.Enrollment).WithMany(x => x.LessonProgress).HasForeignKey(x => x.EnrollmentId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<LessonProgress>().HasOne(x => x.Lesson).WithMany().HasForeignKey(x => x.LessonId).OnDelete(DeleteBehavior.Restrict);
    }
}
