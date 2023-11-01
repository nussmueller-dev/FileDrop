using System.ComponentModel.DataAnnotations;

namespace FileDropBE.BindingModels {
  public class QrLoginBindingModel {
    [Required]
    public string Token { get; set; }
  }
}
