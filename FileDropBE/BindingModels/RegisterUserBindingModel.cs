using System.ComponentModel.DataAnnotations;

namespace FileDropBE.BindingModels {
  public class RegisterUserBindingModel {
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public bool Remember { get; set; }
  }
}
