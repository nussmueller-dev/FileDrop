using FileDropBE.Database.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileDropBE.BindingModels {
  public class BindingModelFactory {
    //public static User GetUserFromBindingModel(RegisterBindingModel model) {
    //  User user = new User();

    //  user.Username = model.Username;
    //  user.Email = model.Email;
    //  user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

    //  return user;
    //}

    public static File GetFileFromForm(IFormFile form) {
      File file = new File();

      file.Name = GetFileName(form);
      file.Date = DateTime.Now;
      file.FileType = GetFileType(form);
      file.MimeType = form.ContentType;
      file.Size = form.Length;

      return file;
    }

    private static string GetFileName(IFormFile form) {
      var nameSplit = form.FileName.Split('.').ToList();
      nameSplit.Remove(nameSplit.Last());
      return string.Join('.', nameSplit);
    }

    private static string GetFileType(IFormFile form) {
      var nameSplit = form.FileName.Split('.').ToList();
      return "." + nameSplit.Last();
    }
  }
}
