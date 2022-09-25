using FileDropBE.Database;
using FileDropBE.Database.Entities;
using FileDropBE.Logic;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace FileDropBE.BindingModels {
  public class BindingModelFactory {
    private readonly UserLogic _userLogic;
    private readonly DB_Context _context;

    public BindingModelFactory(UserLogic userLogic, DB_Context context) {
      _userLogic = userLogic;
      _context = context;
    }

    public User GetUserFromRegisterBindingModel(RegisterUserBindingModel model) {
      User user = new User();

      user.Username = model.Username;
      user.Salt = _userLogic.GetSalt();
      user.PasswordHash = _userLogic.HashPassword(model.Password, user.Salt);

      return user;
    }

    public User GetUserFromLoginBindingModel(LoginUserBindingModel model) {
      var user = _context.Users.FirstOrDefault(x => x.Username == model.Username);

      if (user is null) {
        return null;
      }

      var passwordHash = _userLogic.HashPassword(model.Password, user.Salt);

      if (user.PasswordHash == passwordHash) {
        return null;
      }

      return user;
    }

    public File GetFileFromForm(IFormFile form) {
      File file = new File();

      file.Name = GetFileName(form);
      file.Date = DateTime.Now;
      file.FileType = GetFileType(form);
      file.MimeType = form.ContentType;
      file.Size = form.Length;

      return file;
    }

    private string GetFileName(IFormFile form) {
      var nameSplit = form.FileName.Split('.').ToList();
      nameSplit.Remove(nameSplit.Last());
      return string.Join('.', nameSplit);
    }

    private string GetFileType(IFormFile form) {
      var nameSplit = form.FileName.Split('.').ToList();
      return "." + nameSplit.Last();
    }
  }
}
