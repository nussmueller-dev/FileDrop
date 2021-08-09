using FileDropBE.BindingModels;
using FileDropBE.Database;
using FileDropBE.Database.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileDropBE.Logic {
  public class FileLogic {
    const string uploadPath = "uploads";
    const string fileNamePrefix = "file";

    public int SaveFile(DB_Context context, IFormFile form) {
      Database.Entities.File file;
      var fileName = fileNamePrefix;

      file = BindingModelFactory.GetFileFromForm(form);
      context.Files.Add(file);
      context.SaveChanges();

      fileName += file.Id;
      fileName += file.FileType;
      var path = SaveFileToPath(form, fileName);
      file.Path = path;
      context.SaveChanges();

      return file.Id;
    }

    public void DeleteFile(DB_Context context, Database.Entities.File file) {
      var files = context.Files;

      System.IO.File.Delete(file.Path);

      files.Remove(file);
      context.SaveChanges();
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
