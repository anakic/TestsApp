using jogging.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace jogging
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JoggingDbContext _context;

        public AuthenticationMiddleware(RequestDelegate next, JoggingDbContext context)
        {
            _next = next;
            _context = context;
        }

        public async Task Invoke(HttpContext context)
        {
            //if no cookie authentication, try basic http authentication (non-broswer clients, e.g. applications, postman etc...)
            if (context.User.Identity.Name == null)
            {
                var authHeader = context.Request.Headers["Authorization"].SingleOrDefault();
                if (authHeader != null && authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authHeader.Substring("Basic ".Length).Trim();
                    var credentialstring = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                    var credentials = credentialstring.Split(':');
                    string email = credentials[0];
                    string password = credentials[1];

                    var user = _context.Users.FindByEmail(email);
                    if (user != null && user.IsPasswordValid(password))
                    {
                        var claims = new[] {
                                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                                new Claim(ClaimTypes.Role, user.Role.ToString()),
                                new Claim(ClaimTypes.Name, user.Email)
                            };
                        var identity = new ClaimsIdentity(claims, "Basic");
                        context.User = new ClaimsPrincipal(identity);
                    }
                }
            }

            await _next.Invoke(context);
        }
    }
}
