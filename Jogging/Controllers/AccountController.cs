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
    public class AccountController : Controller
    {
        IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_userService.GetCurrentUser());
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginCredentials credentials)
        {
            try
            {
                var user = await _userService.LoginAsync(credentials.Email, credentials.Password);
                return Ok(user);
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public class LoginCredentials
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }
    }
}
