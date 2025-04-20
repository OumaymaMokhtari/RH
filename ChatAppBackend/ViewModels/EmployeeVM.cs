using System.ComponentModel.DataAnnotations;

namespace ChatAppBackend.ViewModels;

public class EmployeeVM
{
    public string Id { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire.")]
    public string Password { get; set; }  

    public string Role { get; set; }

    [Required(ErrorMessage = "L'Username est obligatoire.")]
    public string Username { get; set; }  
}
