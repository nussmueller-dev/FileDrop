using FileDropBE.BindingModels;
using FileDropBE.Database;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace FileDropBE.Logic {
  public class FileLogic {
    const string uploadPath = "uploads";
    const string fileNamePrefix = "file";

    private readonly DB_Context _dbContext;

    public FileLogic(DB_Context dB_Context) {
      _dbContext = dB_Context;
    }

    public int SaveFile(IFormFile form) {
      Database.Entities.File file;
      var fileName = fileNamePrefix;

      file = BindingModelFactory.GetFileFromForm(form);
      _dbContext.Files.Add(file);
      _dbContext.SaveChanges();

      fileName += file.Id;
      fileName += file.FileType;
      var path = SaveFileToPath(form, fileName);
      file.Path = path;
      _dbContext.SaveChanges();

      return file.Id;
    }

    public void DeleteFile(Database.Entities.File file) {
      var files = _dbContext.Files;

      File.Delete(file.Path);

      files.Remove(file);
      _dbContext.SaveChanges();
    }

    private string SaveFileToPath(IFormFile form, string fileName) {
      string filePath = Path.Combine(uploadPath, fileName);

      Directory.CreateDirectory(uploadPath);

      using (Stream fileStream = new FileStream(filePath, FileMode.Create)) {
        form.CopyTo(fileStream);
      }
      return Path.GetFullPath(filePath);
    }
  }
}
