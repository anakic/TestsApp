using jogging.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace jogging.Services
{
    public class UserService : IUserService
    {
        public TimeSpan CookieDuration { get; set; } = new TimeSpan(14, 0, 0, 0, 0);//14 days
        IHttpContextAccessor _httpContextAccessor;
        JoggingDbContext _context;
        public UserService(IHttpContextAccessor httpContextAccessor, JoggingDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public User GetCurrentUser()
        {
            return _context.Users.FindByEmail(GetCurrentUserIdentity());
        }

        public string GetCurrentUserIdentity()
        {
            return _httpContextAccessor.HttpContext.User?.Identity?.Name;
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            var myUser = _context.Users.FindByEmail(email);
            if (myUser == null || myUser.IsPasswordValid(password))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            else
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, myUser.Email), new Claim(ClaimTypes.Role, myUser.Role.ToString()) };

                var props = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.Add(CookieDuration)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await _httpContextAccessor.HttpContext.Authentication.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), props);
                return myUser;
            }
        }
    }
}
