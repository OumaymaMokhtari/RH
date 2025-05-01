using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;
using GestionConges.Data;
using GestionConges.Models;

namespace GestionConges.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScannerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ScannerController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ImageRequest request)
        {
            try
            {
                Console.WriteLine("Requête reçue.");

                // Nettoyage de l’image base64
                var base64 = Regex.Replace(request.ImageBase64, "^data:image\\/[a-zA-Z]+;base64,", string.Empty);
                var bytes = Convert.FromBase64String(base64);

                // Enregistrement local de l'image
                var dossier = @"C:\Users\oumayma\source\repos\ReconnaissanceAbsence\captures";
                if (!Directory.Exists(dossier))
                {
                    Directory.CreateDirectory(dossier);
                    Console.WriteLine("Dossier créé : " + dossier);
                }

                var cheminImage = Path.Combine(dossier, "aujourdhui.jpg");
                System.IO.File.WriteAllBytes(cheminImage, bytes);
                Console.WriteLine("Image enregistrée à : " + cheminImage);

                // Préparation du processus Python
                var psi = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"\"C:\\Users\\oumayma\\source\\repos\\ReconnaissanceAbsence\\reconnaissance_api.py\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                Console.WriteLine("Lancement du script Python...");
                var process = Process.Start(psi);
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                Console.WriteLine("Sortie brute Python : " + output);
                Console.WriteLine("Erreur éventuelle Python : " + error);

                var statut = output.Trim();

                if (string.IsNullOrWhiteSpace(statut))
                {
                    Console.WriteLine("Aucun statut retourné par le script Python !");
                }
                else
                {
                    Console.WriteLine("Statut final : " + statut);
                }

                // Traitement selon le statut
                if (statut.StartsWith("absent:"))
                {
                    int id = ExtraireId(statut);
                    Console.WriteLine("Absence détectée pour Employé ID : " + id);

                    var absence = new Absence
                    {
                        EmployeId = id,
                        DateAbsence = DateTime.Now,
                        Raison = "Absence détectée automatiquement"
                    };

                    _context.Absences.Add(absence);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Absence enregistrée en base.");
                }
                else if (statut.StartsWith("present:") || statut.StartsWith("retard:"))
                {
                    int id = ExtraireId(statut);
                    Console.WriteLine("Présence ou retard détecté pour Employé ID : " + id);
                    // Pas d'enregistrement nécessaire
                }
                else
                {
                    Console.WriteLine("Format inattendu reçu : " + statut);
                }

                return Ok(new { message = "Image reçue", statut = statut });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception attrapée : " + ex.Message);
                return StatusCode(500, $"Erreur : {ex.Message}");
            }
        }

        private int ExtraireId(string statut)
        {
            var parts = statut.Split(':');
            if (parts.Length > 1 && int.TryParse(parts[1], out var id))
            {
                Console.WriteLine("ID extrait avec succès : " + id);
                return id;
            }

            Console.WriteLine("ID non extrait. Chaîne reçue : " + statut);
            return 0;
        }

        public class ImageRequest
        {
            public string ImageBase64 { get; set; }
        }
    }
}
