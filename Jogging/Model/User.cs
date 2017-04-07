using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jogging.Model
{
    public enum UserRole
    {
        User,
        Manager,
        Admin
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; }

        public IEnumerable<Entry> Entries { get; set; }
    }
}
