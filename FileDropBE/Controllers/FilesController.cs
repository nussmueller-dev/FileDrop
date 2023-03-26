using FileDropBE.Attributes;
using FileDropBE.Database;
using FileDropBE.Logic;
using FileDropBE.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FileDropBE.Controllers {
  [Route("files")]
  [ApiController]
  public class FilesController : ControllerBase {
    private const long MaxFileSize = 50L * 1024L * 1024L * 1024L; // 50GB, adjust to your need
    private DB_Context _context;
    private FileLogic _logic;
    private IConfiguration _configuration;

    public FilesController(IConfiguration configuration, DB_Context context, FileLogic logic) {
      _context = context;
      _logic = logic;
      _configuration = configuration;
    }

    [RequestSizeLimit(MaxFileSize)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
    [HttpPost("upload")]
    public IActionResult UploadFile([FromForm] IList<IFormFile> file) {
      foreach (var singleFile in file) {
        var fileId = _logic.SaveFile(singleFile);
      }

      if (file.Count() == 0) {
        return BadRequest("No files");
      } else {
        return Ok();
      }
    }

    [RequestSizeLimit(MaxFileSize)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
    [HttpPost("upload/share")]
    public IActionResult UploadFileAsShare([FromForm] IList<IFormFile> file) {
      var fileTitles = new List<string>();

      foreach (var singleFile in file) {
        var fileId = _logic.SaveFile(singleFile);

        var fileEntity = _context.Files.First(x => x.Id == fileId);

        fileTitles.Add(fileEntity.Name + fileEntity.FileType);
      }

#if (DEBUG)
      var frontendUrl = _configuration["FrontendUrl:Debug"];
#else
      var frontendUrl = "/";
#endif

      if (file.Count() == 0) {
        return BadRequest("No files");
      }

      return Redirect($"{frontendUrl}upload?filetitles={String.Join("&filetitles=", fileTitles)}");
    }

    [Authorize]
    [HttpGet("")]
    public IActionResult GetAllFiles() {
      var allFiles = _logic.GetAllFiles();

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
