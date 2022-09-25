﻿using FileDropBE.BindingModels;
using FileDropBE.Database;
using FileDropBE.Logic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;


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

    [HttpPost("register")]
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
    public IActionResult LoginUser(LoginUserBindingModel bindingModel) {
      var user = _bindingModelFactory.GetUserFromLoginBindingModel(bindingModel);

      if (user == null) { 
        return Unauthorized();
      }

      return Ok(_userLogic.BuildToken(user));
    }
  }
}
