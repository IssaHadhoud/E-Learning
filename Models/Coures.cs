using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace E_learning.Models;

public class Courses
{
    [Key] public Guid Id { get; set; }
    [Required] public string Title { get; set; }
    [Required] public string Details { get; set; }
    public string VideoURL { get; set; }

    public string Documents { get; set; }
    public string ImageURL { get; set; }

    public string Assignment { get; set; }


}