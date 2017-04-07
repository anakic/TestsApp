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
        JoggingDbContext _context;
        public UserService(IHttpContextAccessor httpContextAccessor, JoggingDbContext context)
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
