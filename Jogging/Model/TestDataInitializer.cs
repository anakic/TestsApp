using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jogging.Model
{
    public class TestDatanitializer
    {
        static Random _rng = new Random();

        public static void Initialize(JoggingDbContext ctx)
        {
            ctx.Database.EnsureDeleted();
            ctx.Database.EnsureCreated();

            var user = new User() { Id = 0, Email = "regular@user.com", FirstName = "Peon", LastName = "User", Role = UserRole.User };
            user.SetPassword("pwd");
            ctx.Add(user);

            var manager = new User() { Id = 0, Email = "manager@user.com", FirstName = "Bishop", LastName = "User", Role = UserRole.Manager };
            manager.SetPassword("pwd");
            ctx.Add(manager);

            var admin = new User() { Id = 0, Email = "admin@user.com", FirstName = "King", LastName = "User", Role = UserRole.Admin };
            admin.SetPassword("pwd");
            ctx.Add(admin);

            ctx.SaveChanges();

            IEnumerable<Entry> _entries = _entries = Enumerable.Range(1, 500)
                .Where(r => _rng.Next(7) != 1)
                .Select(index => new Entry()
                {
                    Id = 0,
                    User = user,
                    Date = DateTime.Now.AddDays(-index),
                    TimeInSeconds = _rng.Next(40 * 60, 55 * 60),
                    DistanceInMeters = _rng.Next(8000, 12000)
                });

            
            ctx.AddRange(_entries);
            ctx.SaveChanges();
        }
    }
}
