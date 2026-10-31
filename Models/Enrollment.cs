namespace E_learning.Models;

public class Enrollment
{
    public int Id { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public ApplicationUser Student { get; set; } = null!;
    public Guid CourseId { get; set; }
    public Courses Course { get; set; } = null!;
    public DateTime EnrolledUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedUtc { get; set; }
    public ICollection<LessonProgress> LessonProgress { get; set; } = new List<LessonProgress>();
}
