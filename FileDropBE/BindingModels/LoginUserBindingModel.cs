using System.ComponentModel.DataAnnotations;

namespace FileDropBE.BindingModels {
  public class LoginUserBindingModel {
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public bool Remember { get; set; }
  }
}
