using jogging.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogging.Tests.Mocks
{
    class DummyUserService : IUserService
    {
        string _userToReturn;

        public DummyUserService(string userToReturn)
        {
            SetUser(userToReturn);
        }

        public void SetUser(string userToReturn)
        {
            _userToReturn = userToReturn;
        }

        public string GetCurrentUserIdentity()
        {
            return _userToReturn;
        }
    }
}
