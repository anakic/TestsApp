using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using jogging.Model;
using jogging.Services;

namespace jogging.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        ILoginService _userService;
        public UsersController(ILoginService userService)
        {
            _userService = userService;
        }

        [HttpGet()]
        public IActionResult Current()
        {
            var user = _userService.GetCurrentUser();
            if (user != null)
                return Ok(user);
            else
                return NotFound();
        }

        [HttpPost()]
        public async Task<IActionResult> Login([FromBody]LoginCredentials credentials)
        {
            var user = await _userService.LoginAsync(credentials.Email, credentials.Password);
            if (user != null)
                return Ok(user);
            else
                return NotFound("Invalid credentials.");
        }

        [HttpDelete()]
        public async Task<IActionResult> SignOut()
        {
            await _userService.SignOut();
            return Ok();
        }

        public class LoginCredentials
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
