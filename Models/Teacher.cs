using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_learning.Models;

public class Teacher: Users
{
    public Guid CoursesId { get; set; }
    [ForeignKey("CoursesId")]
    [DeleteBehavior(DeleteBehavior.NoAction)]
    
   public Courses Courses { get; set; } = null!;
   

   

}
