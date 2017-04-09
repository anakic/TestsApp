using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace jogging.Services
{
    public class EmailNotifier : IEmailNotifier
    {
        public void SendUserdDetails(string email, string firstName, string lastaName, string password, string role)
        {
            Debug.WriteLine($"Sending email to {email}: first name: {firstName}, last name: {lastaName}, role: {role}, password: {password}, role: {role}");
        }
    }
}
