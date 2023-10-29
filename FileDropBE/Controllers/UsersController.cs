using FileDropBE.BindingModels;
using FileDropBE.Database;
using FileDropBE.Logic;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using System.Linq;
using IronBarCode;
using static System.Net.Mime.MediaTypeNames;
using static IronSoftware.Drawing.AnyBitmap;


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
    public IActionResult GetUsersCount() {
      return Ok(_context.Users.Count());
    }

    [HttpGet("qr-login-code")]
    public IActionResult GetQrLoginCode() {
      var qrCode = QRCodeWriter.CreateQrCode("https://nussmueller.dev", 300, QrVersion: 4);
      var qrCodeImage = qrCode.ToBitmap();

      using (var stream = qrCodeImage.ToStream(ImageFormat.Jpeg)) {
        var byteArray = stream.ToArray();
        return File(byteArray, "image/jpeg");
      }
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

      return Ok(_userLogic.BuildToken(user));
    }

    [HttpPost("login")]
    [Produces("application/json")]
    public IActionResult LoginUser(LoginUserBindingModel bindingModel) {
      var user = _bindingModelFactory.GetUserFromLoginBindingModel(bindingModel);

      if (user == null) { 
        return Unauthorized();
      }

      return Ok(_userLogic.BuildToken(user));
    }
  }
}
