using E_learning.Models;

namespace E_learning.ViewModel;

public class LessonLearningViewModel
{
    public Courses Course { get; set; } = null!;
    public Lesson Lesson { get; set; } = null!;
    public int? PreviousLessonId { get; set; }
    public int? NextLessonId { get; set; }
    public bool IsCompleted { get; set; }
    public int ProgressPercent { get; set; }
}
