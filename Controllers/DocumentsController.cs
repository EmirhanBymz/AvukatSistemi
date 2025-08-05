using AvukatSistemi.Data;
using Microsoft.AspNetCore.Mvc;

public class DocumentsController : Controller
{
    private readonly ApplicationDbContext _context;

    public DocumentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Download(int id)
    {
        var document = await _context.Documents.FindAsync(id);
        if (document == null)
            return NotFound();

        // Örnek olarak dosya yolunu ve mime türünü burada kullan
        var path = Path.Combine("wwwroot/files", document.FilePath);
        var contentType = "application/octet-stream"; // Dosya türüne göre ayarla

        var bytes = await System.IO.File.ReadAllBytesAsync(path);
        return File(bytes, contentType, document.FileName);
    }
}
