﻿using System;

namespace FileDropBE.Database.Entities {
  public class File {
    public int Id { get; set; }
    public string Name { get; set; }
    public string FileType { get; set; }
    public string MimeType { get; set; }
    public string Path { get; set; }
    public double Size { get; set; }
    public DateTime Date { get; set; }
    public bool VirusProofed { get; set; } = false;
    public bool VirusFound { get; set; }
  }
}
