using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using Microsoft.EntityFrameworkCore;

namespace E_learning.Models;

public class Student : Users
{
    public Guid CoursesId { get; set; }

    [ForeignKey("CoursesId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    public Courses Courses { get; set; } = null!;

    public string Schema { get; set; } = string.Empty;
    public int StudyHours { get; set; }
    public int Marks { get; set; }
}
