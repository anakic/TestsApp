using jogging.Model;
using jogging.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogging.Tests.Mocks
{
    class DummyUserService : IUserService
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
    }
}
