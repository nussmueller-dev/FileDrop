using FileDropBE.Database.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace FileDropBE.BindingModels {
  public class AcceptQrLoginBindingModel {
    [Required]
    public string Token { get; set; }
    [Required]
    public int? SessionVallidForHours { get; set; }
    [JsonIgnore]
    public User User { get; set; }
  }
}
