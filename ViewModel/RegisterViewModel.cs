using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic.CompilerServices;

namespace E_learning.ViewModel;

public class RegisterViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string PassWord { get; set; }
    [Required]
    [Compare("PassWord")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
    public string FirstName { get; set; }
 
    public string LastName { get; set; }
    public string Department { get; set; }    
}