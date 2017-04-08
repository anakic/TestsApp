using jogging.Model;
using jogging.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jogging.Tests.Mocks
{
    class DummyUserService : ILoginService
    {
        User _userToReturn;

        public DummyUserService(User userToReturn)
        {
            SetUser(userToReturn);
        }

        public void SetUser(User userToReturn)
        {
            _userToReturn = userToReturn;
        }

        public User GetCurrentUser()
        {
            return _userToReturn;
        }

        public Task<User> LoginAsync(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task SignOut()
        {
            throw new NotImplementedException();
        }
    }
}
