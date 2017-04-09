using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using jogging.Model;
using jogging.Services;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace jogging.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = nameof(UserRole.Admin) + ", " + nameof(UserRole.Manager))]
    public class UsersController : Controller
    {
        JoggingDbContext _context;
        ILoginService _loginService;
        IEmailNotifier _emailNotifier;
        public UsersController(ILoginService userService, JoggingDbContext context, IEmailNotifier emailNotifier)
        {
            _loginService = userService;
            _context = context;
            _emailNotifier = emailNotifier;
        }

        [HttpGet()]
        public IActionResult Get(string searchTerm)
        {
            string[] searchSegments = (searchTerm ?? "")
                .Split(new string[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries)
                .Select(seg => seg.ToUpper())
                .ToArray();

            var users = _context.Users
                .Where(u => searchSegments.Length == 0 || searchSegments.All(seg => u.Email.ToUpper().Contains(seg) || u.FirstName.ToUpper().Contains(seg) || u.LastName.ToUpper().Contains(seg)))
                .OrderBy(u => u.Email);

            return Ok(users);
        }

        Regex emailRegex = new Regex(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);

        [HttpPost()]
        public IActionResult Post([FromBody]UserDto newUserDto)
        {
            if (newUserDto.Role > _loginService.GetCurrentUser().Role)
            {
                return BadRequest("Cannot bestow a user with a more magnificent role than current user.");
            }
            else if (emailRegex.IsMatch(newUserDto.Email) == false)
            {
                return BadRequest("Invalid email address.");
            }
            else if (_context.Users.FindByEmail(newUserDto.Email) != null)
            {
                return BadRequest("A user already exists with that email address.");
            }
            else
            {
                var newUser = new User()
                {
                    Email = newUserDto.Email,
                    FirstName = newUserDto.FirstName,
                    LastName = newUserDto.LastName,
                    Role = newUserDto.Role,
                };

                //generate random password user
                string password = Guid.NewGuid().ToString("n").Substring(12);
                newUser.SetPassword(password);
                _context.Add(newUser);
                _context.SaveChanges();

                _emailNotifier.SendUserdDetails(newUser.Email, newUser.FirstName, newUser.LastName, password, newUser.Role.ToString());

                return Ok();
            }
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> Put(int userId, [FromBody]UserDto userDTO)
        {
            var currentUser = _loginService.GetCurrentUser();
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return BadRequest("Invalid userId");
            else if (emailRegex.IsMatch(userDTO.Email) == false)
            {
                return BadRequest("Invalid email address.");
            }
            else if (currentUser.Id == userId && currentUser.Role != userDTO.Role)
            {
                return BadRequest("User cannot modify their own role!");
            }
            else if (userDTO.Role > currentUser.Role || user.Role > currentUser.Role)
            {
                return BadRequest("Cannot bestow a user with a more magnificent role than current user");
            }
            else
            {
                var userWithSpecifiedEmail = _context.Users.FindByEmail(userDTO.Email);
                if (userWithSpecifiedEmail != null && userWithSpecifiedEmail != user)
                {
                    return BadRequest("Anoter user with the specified email already exists.");
                }
                else
                {
                    user.Email = userDTO.Email;
                    user.FirstName = userDTO.FirstName;
                    user.LastName = userDTO.LastName;
                    user.Role = userDTO.Role;
                    _context.SaveChanges();
                    return Ok();
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return BadRequest("Invalid user");
            else if (user.Id == _loginService.GetCurrentUser().Id)
                return BadRequest("User cannot delete own account.");
            else
            {
                _context.Remove(user);
                _context.SaveChanges();
                return Ok();
            }
        }
    }

    public class UserDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; }
    }
}
