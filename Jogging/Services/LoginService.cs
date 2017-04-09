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
    public class LoginService : ILoginService
    {
        public TimeSpan CookieDuration { get; set; } = new TimeSpan(14, 0, 0, 0, 0);//14 days
        IHttpContextAccessor _httpContextAccessor;
        JoggingDbContext _context;
        public LoginService(IHttpContextAccessor httpContextAccessor, JoggingDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public User GetCurrentUser()
        {
            return _context.Users.Find(GetCurrentUserId());
        }

        public int GetCurrentUserId()
        {
            return (_httpContextAccessor.HttpContext.User?.Identity as ClaimsIdentity).Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => int.Parse(c.Value))
                .SingleOrDefault();
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            var myUser = _context.Users.FindByEmail(email);
            if (myUser == null || !myUser.IsPasswordValid(password))
            {
                return null;
            }
            else
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, myUser.Email),
                    new Claim(ClaimTypes.NameIdentifier, myUser.Id.ToString()),
                    new Claim(ClaimTypes.Role, myUser.Role.ToString())
                };

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

        public async Task SignOut()
        {
            await _httpContextAccessor.HttpContext.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
