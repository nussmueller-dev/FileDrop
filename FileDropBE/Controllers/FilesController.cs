using FileDropBE.Attributes;
using FileDropBE.Database;
using FileDropBE.Logic;
using FileDropBE.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FileDropBE.Controllers {
  [Route("files")]
  [ApiController]
  public class FilesController : ControllerBase {
    private const long MaxFileSize = 50L * 1024L * 1024L * 1024L; // 50GB, adjust to your need
    private DB_Context _context;
    private FileLogic _logic;

    public FilesController(DB_Context context, FileLogic logic) {
      _context = context;
      _logic = logic;
    }

    [RequestSizeLimit(MaxFileSize)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
    [HttpPost("upload")]
    public IActionResult UploadFile([FromForm] IFormFile file) {
      var fileId = _logic.SaveFile(file);

      return Ok(new { id = fileId });
    }

    [Authorize]
    [HttpGet("")]
    public IActionResult GetAllFiles() {
      var allFiles = _context.Files.ToList();

      return Ok(allFiles.Select(x => new FileViewModel(x)));
    }

    [Authorize]
    [HttpGet("{id:int}/download")]
    public IActionResult GetDownload(int id) {
      var file = _context.Files.FirstOrDefault(x => x.Id == id);

      if (file is null) {
        return BadRequest(new { message = "File doesn't exist" });
      }

      return PhysicalFile(file.Path, file.MimeType, file.Name + file.FileType);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public IActionResult DeleteFile(int id) {
      var file = _context.Files.FirstOrDefault(x => x.Id == id);

      if (file is null) {
        return BadRequest(new { message = "File doesn't exist" });
      }

      _logic.DeleteFile(file);

      return Ok();
    }
  }
}
