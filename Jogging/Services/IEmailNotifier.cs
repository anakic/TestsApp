using jogging.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jogging.Services
{
    public interface IEmailNotifier
    {
        void SendUserdDetails(string email, string firstName, string lastaName, string password, string role);
    }
}
