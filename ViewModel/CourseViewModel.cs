using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace E_learning.ViewModel;

public class CourseViewModel
{
    public Guid ID { get; set; }
    [Required(ErrorMessage = "Title is required.")]
    public string Title { get; set; }
    [Required(ErrorMessage = "Details is required.")]
    public string Details { get; set; }
    public IFormFile Video { get; set; }
    public IFormFile Image { get; set; }
    public IFormFile Document { get; set; }
    public IFormFile Assignment { get; set; }
}
