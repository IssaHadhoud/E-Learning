using System.ComponentModel.DataAnnotations;

namespace E_learning.Models;

public class Users
{
    [Key]
    public Guid userID { get; set; }
   
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    
    public string Password { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    
    public DateTime DateOfRegister   { get; set; }
    
    
}
