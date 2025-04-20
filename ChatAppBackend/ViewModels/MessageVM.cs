using System.ComponentModel.DataAnnotations;

namespace ChatAppBackend.ViewModels;

public class MessageVM
{
    [Required(ErrorMessage = "L'expéditeur est obligatoire.")]
    public string SenderId { get; set; }

    [Required(ErrorMessage = "Le destinataire est obligatoire.")]
    public string ReceiverId { get; set; }

    [Required(ErrorMessage = "Le contenu du message est obligatoire.")]
    [MinLength(1, ErrorMessage = "Le message ne peut pas être vide.")]
    public string Content { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
