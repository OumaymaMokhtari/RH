using System.ComponentModel.DataAnnotations;

namespace ChatAppBackend.ViewModels;

public class EmployeeVM
{
    public string Id { get; set; }

    public string EmployeeId { get; set; }

    [Required(ErrorMessage = "Le nom est obligatoire.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
    public string Password { get; set; }

    public string Role { get; set; }

    [Required(ErrorMessage = "L'Username est obligatoire.")]
    public string Username { get; set; }

    public string Cin { get; set; }
    public string DateNaissance { get; set; }
    public string Email { get; set; }
    public string Tele { get; set; }
    public string EmployeIdSql { get; set; }
}
