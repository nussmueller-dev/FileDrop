using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileDropBE.Database.Entities {
  public class Document {
    public int Id { get; set; }
    public string Name { get; set; }
    public string FileType { get; set; }
    public string MimeType { get; set; }
    public string Path { get; set; }
    public double Size { get; set; }
    public DateTime Date { get; set; }
    public bool VirusProofed { get; set; }
    public bool VirusFound { get; set; }
  }
}
