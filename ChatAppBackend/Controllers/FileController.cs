using Microsoft.AspNetCore.Mvc;

namespace ChatAppBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

    public FileController()
    {
        if (!Directory.Exists(_uploadPath))
            Directory.CreateDirectory(_uploadPath);
    }

    // POST : api/File/upload
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Aucun fichier sélectionné.");

        var filePath = Path.Combine(_uploadPath, file.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{file.FileName}";

        return Ok(new { fileUrl });
    }

    // GET : api/File/download/{filename}
    [HttpGet("download/{filename}")]
    public IActionResult DownloadFile(string filename)
    {
        var filePath = Path.Combine(_uploadPath, filename);

        if (!System.IO.File.Exists(filePath))
            return NotFound();

        var contentType = "application/octet-stream";
        return PhysicalFile(filePath, contentType, filename);
    }
}
