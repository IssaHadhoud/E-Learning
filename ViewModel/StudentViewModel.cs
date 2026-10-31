namespace E_learning.ViewModel;

public class StudentViewModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;

    public IFormFile? ImageURL { get; set; }
    
    
}
