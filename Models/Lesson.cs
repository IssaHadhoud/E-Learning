using System.ComponentModel.DataAnnotations;

namespace E_learning.Models;

public class Lesson
{
    public int Id { get; set; }
    public int CourseModuleId { get; set; }
    public CourseModule CourseModule { get; set; } = null!;
    [Required, StringLength(160)] public string Title { get; set; } = string.Empty;
    [StringLength(2000)] public string? Description { get; set; }
    [StringLength(1000)] public string? MediaUrl { get; set; }
    public int DurationMinutes { get; set; }
    public int SortOrder { get; set; }
    public bool IsPublished { get; set; }
}
