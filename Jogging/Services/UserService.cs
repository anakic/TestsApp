using jogging.Model;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jogging.Services
{
    public class UserService : IUserService
    {
        IHttpContextAccessor _httpContextAccessor;
        JoggingContext _context;
        public UserService(IHttpContextAccessor httpContextAccessor, JoggingContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public string GetCurrentUserIdentity()
        {
            return _httpContextAccessor.HttpContext.User?.Identity?.Name;
        }
    }
}
