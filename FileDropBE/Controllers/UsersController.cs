using FileDropBE.BindingModels;
using FileDropBE.Database;
using FileDropBE.Logic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using FileDropBE.Attributes;
using QRCoder;
using System.IO;
using SixLabors.ImageSharp.Formats.Jpeg;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileDropBE.Controllers {
  [Route("users")]
  [ApiController]
  public class UsersController : ControllerBase {
    private DB_Context _context;
    private BindingModelFactory _bindingModelFactory;
    private UserLogic _userLogic;

    public UsersController(DB_Context context, BindingModelFactory bindingModelFactory, UserLogic userLogic) {
      _context = context;
      _bindingModelFactory = bindingModelFactory;
      _userLogic = userLogic;
    }

    [HttpGet("count")]
    [Produces("application/json")]
    public IActionResult GetUsersCount() {
      return Ok(_context.Users.Count());
    }

    [HttpGet("qrlogin-code")]
    [Produces("application/json")]
    public IActionResult GetQrLoginCode() {
      return Ok(_userLogic.CreateQrLogin());
    }

    [HttpGet("qr-code")]
    public IActionResult GetQrCode([FromQuery] string url) {
      using QRCodeGenerator qrGenerator = new QRCodeGenerator();
      using QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
      using QRCode qrCode = new QRCode(qrCodeData);

      SixLabors.ImageSharp.Image image = qrCode.GetGraphic(10);

      using MemoryStream stream = new MemoryStream();
      image.Save(stream, new JpegEncoder());

      return File(stream.ToArray(), "image/jpeg");
    }

    [Authorize]
    [HttpPost("accept-qrlogin")]
    public IActionResult AcceptQrLogin([FromBody] AcceptQrLoginBindingModel model) {
      if (_userLogic.AcceptQrLogin(model)) {
        return Ok();
      } else {
        return BadRequest();
      }
    }

    [HttpPost("login/qr")]
    [Produces("application/json")]
    public IActionResult LoginUserQr([FromBody] QrLoginBindingModel model) {
      var acceptedModel = _userLogic.GetAcceptedQrLoginModelFromToken(model.Token);

      if (acceptedModel == null) {
        return Unauthorized();
      }

      return Ok(_userLogic.BuildToken(acceptedModel.User, acceptedModel.SessionVallidForHours));
    }

    [HttpPost("register")]
    [Produces("application/json")]
    public IActionResult RegisterUser(RegisterUserBindingModel bindingModel) {
      if (_context.Users.Count() > 0) {
        return BadRequest("There already is a User");
      }

      var user = _bindingModelFactory.GetUserFromRegisterBindingModel(bindingModel);
      _context.Users.Add(user);
      _context.SaveChanges();

      return Ok(_userLogic.BuildToken(user, bindingModel.Remember ? 0 : null));
    }

    [HttpPost("login")]
    [Produces("application/json")]
    public IActionResult LoginUser(LoginUserBindingModel bindingModel) {
      var user = _bindingModelFactory.GetUserFromLoginBindingModel(bindingModel);

      if (user == null) { 
        return Unauthorized();
      }

      return Ok(_userLogic.BuildToken(user, bindingModel.Remember ? 0 : null));
    }
  }
}
