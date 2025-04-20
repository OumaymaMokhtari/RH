using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace GestionConges.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScannerController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromBody] ImageRequest request)
        {
            try
            {
                var base64 = Regex.Replace(request.ImageBase64, "^data:image\\/[a-zA-Z]+;base64,", string.Empty);
                var bytes = Convert.FromBase64String(base64);

                var dossier = @"C:\Users\oumayma\source\repos\ReconnaissanceAbsence\captures";
                if (!Directory.Exists(dossier))
                    Directory.CreateDirectory(dossier);

                var cheminImage = Path.Combine(dossier, "aujourdhui.jpg");
                System.IO.File.WriteAllBytes(cheminImage, bytes);

                var psi = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"\"C:\\Users\\oumayma\\source\\repos\\ReconnaissanceAbsence\\reconnaissance_absence.py\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                var process = Process.Start(psi);
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                return Ok(new { message = "Image reçue", statut = output.Trim() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erreur : {ex.Message}");
            }
        }

        public class ImageRequest
        {
            public string ImageBase64 { get; set; }
        }
    }
}
