using System.ComponentModel.DataAnnotations;

namespace E_learning.Models;

public class CourseModule
{
    public int Id { get; set; }
    public Guid CourseId { get; set; }
    public Courses Course { get; set; } = null!;
    [Required, StringLength(160)] public string Title { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
