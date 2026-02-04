using System.ComponentModel.DataAnnotations;

namespace E_learning.Models;

public class Users
{
    [Key]
    public Guid userID { get; set; }
   
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    
    public string Password { get; set; }
    public string Department { get; set; }
    
    public DateTime DateOfRegister   { get; set; }
    
    
}