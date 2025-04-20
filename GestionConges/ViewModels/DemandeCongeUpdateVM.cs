using System.ComponentModel.DataAnnotations;
using GestionConges.Filters;

namespace GestionConges.ViewModels
{
    public class DemandeCongeUpdateVM
    {
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public string TypeConge { get; set; }
        public string? Commentaire { get; set; }
    }
}
