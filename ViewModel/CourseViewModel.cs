using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace E_learning.ViewModel;

public class CourseViewModel
{
    public Guid ID { get; set; }
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(160)] public string Title { get; set; } = string.Empty;
    [Required(ErrorMessage = "Details is required.")]
    [StringLength(5000)] public string Details { get; set; } = string.Empty;
    public IFormFile? Video { get; set; }
    public IFormFile? Image { get; set; }
    public IFormFile? Document { get; set; }
    public IFormFile? Assignment { get; set; }
}
