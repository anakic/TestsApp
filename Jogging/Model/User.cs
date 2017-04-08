using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
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

        public bool CanCrudUsers
        {
            get
            {
                return Role != UserRole.User;
            }
        }

        public bool CanCrudAllEntries
        {
            get
            {
                return Role == UserRole.Admin;
            }
        }

        [IgnoreDataMember]
        public byte[] PasswordHash { get; set; }
        [IgnoreDataMember]
        public string Salt { get; set; } = Guid.NewGuid().ToString();

        public IEnumerable<Entry> Entries { get; set; }

        public bool CanAccessEntriesForUser(int targetUserId)
        {
            return (Id == targetUserId || CanCrudAllEntries);
        }

        public bool IsPasswordValid(string password)
        {
            return ByteArrayCompare(GetPasswordHash(password ?? ""), PasswordHash);
        }

        public void SetPassword(string password)
        {
            PasswordHash = GetPasswordHash(password);
        }

        private byte[] GetPasswordHash(string password)
        {
            return MD5.Create().ComputeHash(Encoding.Unicode.GetBytes(password + Salt));
        }

        private static bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;

            return true;
        }
    }
}
