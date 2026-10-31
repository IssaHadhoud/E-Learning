namespace E_learning.Models;

public class LessonProgress
{
    public int Id { get; set; }
    public int EnrollmentId { get; set; }
    public Enrollment Enrollment { get; set; } = null!;
    public int LessonId { get; set; }
    public Lesson Lesson { get; set; } = null!;
    public DateTime? CompletedUtc { get; set; }
    public DateTime LastAccessedUtc { get; set; } = DateTime.UtcNow;
}
