using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic.CompilerServices;

namespace E_learning.ViewModel;

public class RegisterViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
    [Required]
    [Compare("Password")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
    [Required, StringLength(80)] public string FirstName { get; set; } = string.Empty;
 
    [Required, StringLength(80)] public string LastName { get; set; } = string.Empty;
    [StringLength(120)] public string? Department { get; set; }
}
