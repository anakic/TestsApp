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

            var user = new User() { Id = 0, Email = "test@user.com", FirstName = "Test", LastName = "User", Role = UserRole.Manager };
            ctx.Add(user);
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
