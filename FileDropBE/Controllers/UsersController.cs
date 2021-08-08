using FileDropBE.Database;
using FileDropBE.Database.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileDropBE.Controllers {
  [Route("users")]
  [ApiController]
  public class UsersController : ControllerBase {
    private DB_Context _context;

    public UsersController(DB_Context context) {
        _context = context;
    }

    // GET: api/<UserController>
    [HttpGet("test")]
    public IEnumerable<string> Get() {
      _context.Users.Add(new User() { Email = "test@gmail.com", Username = "Niggo", IsOwner = false, PasswordHash = "aösodihf" });
      _context.SaveChanges();
      return new string[] { "value1", "value2" };
    }

    // GET api/<UserController>/5
    [HttpGet("{id}")]
    public string Get(int id) {
      return "value";
    }

    // POST api/<UserController>
    [HttpPost]
    public void Post([FromBody] string value) {
    }

    // PUT api/<UserController>/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value) {
    }

    // DELETE api/<UserController>/5
    [HttpDelete("{id}")]
    public void Delete(int id) {
    }
  }
}
