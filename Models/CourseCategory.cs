using System.ComponentModel.DataAnnotations;

namespace E_learning.Models;

public class CourseCategory
{
    public int Id { get; set; }
    [Required, StringLength(80)] public string Name { get; set; } = string.Empty;
    [StringLength(100)] public string Slug { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    public CourseCategory? ParentCategory { get; set; }
    public ICollection<Courses> Courses { get; set; } = new List<Courses>();
}
