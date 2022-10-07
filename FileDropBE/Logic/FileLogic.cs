using FileDropBE.BindingModels;
using FileDropBE.Database;
using FileDropBE.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileDropBE.Logic {
  public class FileLogic {
    const string uploadPath = "uploads";
    const string fileNamePrefix = "file";

    private readonly DB_Context _dbContext;
    private readonly BindingModelFactory _bindingModelFactory;
    private readonly IHubContext<UploadHub> _uploadHub;

    public FileLogic(DB_Context dB_Context, BindingModelFactory bindingModelFactory, IHubContext<UploadHub> uploadHub) {
      _dbContext = dB_Context;
      _bindingModelFactory = bindingModelFactory;
      _uploadHub = uploadHub;
    }

    public IList<Database.Entities.File> GetAllFiles() {
      if (!Directory.Exists(uploadPath) || Directory.GetFiles(uploadPath).ToList().Count() == 0) {
        _dbContext.Files.RemoveRange(_dbContext.Files);
        _dbContext.SaveChanges();
      }

      var files = _dbContext.Files.ToList();
      return files;
    }

    public int SaveFile(IFormFile form) {
      Database.Entities.File file;
      var fileName = fileNamePrefix;

      file = _bindingModelFactory.GetFileFromForm(form);
      _dbContext.Files.Add(file);
      _dbContext.SaveChanges();

      fileName += file.Id;
      fileName += file.FileType;
      var path = SaveFileToPath(form, fileName);
      file.Path = path;

      while (_dbContext.Files.Count() > 100) {
        var lastFile = _dbContext.Files.Last();

        DeleteFile(lastFile);
      }

      _dbContext.SaveChanges();

      InformAboutNewUpload();

      return file.Id;
    }

    public void DeleteFile(Database.Entities.File file) {
      var files = _dbContext.Files;

      File.Delete(file.Path);

      files.Remove(file);
      _dbContext.SaveChanges();

      InformAboutDeleted();
    }

    private string SaveFileToPath(IFormFile form, string fileName) {
      string filePath = Path.Combine(uploadPath, fileName);

      Directory.CreateDirectory(uploadPath);

      using (Stream fileStream = new FileStream(filePath, FileMode.Create)) {
        form.CopyTo(fileStream);
      }
      return Path.GetFullPath(filePath);
    }

    private void InformAboutNewUpload() {
      _uploadHub.Clients.All.SendAsync("NewUpload");
    }

    private void InformAboutDeleted() {
      _uploadHub.Clients.All.SendAsync("Deleted");
    }
  }
}
