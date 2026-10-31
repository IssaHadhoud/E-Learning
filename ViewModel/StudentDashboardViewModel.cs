namespace E_learning.ViewModel;

public class StudentDashboardViewModel
{
    public string FirstName { get; set; } = string.Empty;
    public IReadOnlyList<EnrolledCourseViewModel> Courses { get; set; } = [];
}

public class EnrolledCourseViewModel
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int? FirstLessonId { get; set; }
    public int CompletedLessons { get; set; }
    public int TotalLessons { get; set; }
    public int ProgressPercent => TotalLessons == 0 ? 0 : (int)Math.Round(100d * CompletedLessons / TotalLessons);
}
