using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_learning.Models;

public class Courses
{
    [Key] public Guid Id { get; set; }
    [Required] public string Title { get; set; } = string.Empty;
    [Required, StringLength(5000)] public string Details { get; set; } = string.Empty;
    [StringLength(180)] public string? Slug { get; set; }
    public string? VideoURL { get; set; }

    public string? Documents { get; set; }
    public string? ImageURL { get; set; }

    public string? Assignment { get; set; }

    public string? TeacherId { get; set; }
    public ApplicationUser? Teacher { get; set; }
    public int? CategoryId { get; set; }
    public CourseCategory? Category { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
    [Precision(10, 2)] public decimal Price { get; set; }
    public ICollection<CourseModule> Modules { get; set; } = new List<CourseModule>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();


}
