using FileDropBE.Database.Entities;
using System;

namespace FileDropBE.ViewModels {
  public class FileViewModel {
    public int Id { get; set; }
    public string Name { get; set; }
    public string FileType { get; set; }
    public string MimeType { get; set; }
    public DateTime Date { get; set; }

    public FileViewModel(File file) {
      Id = file.Id;
      Name = file.Name;
      FileType = file.FileType;
      MimeType = file.MimeType;
      Date = file.Date;
    }
  }
}
