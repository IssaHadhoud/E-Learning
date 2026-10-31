using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace E_learning.Models;

public class ApplicationUser : IdentityUser
{
    [Required, StringLength(80)] public string FirstName { get; set; } = string.Empty;
    [Required, StringLength(80)] public string LastName { get; set; } = string.Empty;
    [StringLength(120)] public string? Department { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
