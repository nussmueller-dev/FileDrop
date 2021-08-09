using FileDropBE.Database;
using FileDropBE.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
      var fileId = _logic.SaveFile(_context, file);

      return Ok(new { id = fileId });
    }
  }
}
