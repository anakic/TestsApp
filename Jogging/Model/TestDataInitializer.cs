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
            ctx.RemoveRange(ctx.Entries);
            ctx.Users.RemoveRange(ctx.Users);
            ctx.SaveChanges();

            var user = new User() { Id = 0, Email = "antonio@jogging.com", FirstName = "Antonio", LastName = "Nakic", Role = UserRole.Manager };
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
