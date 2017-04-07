using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jogging.Model
{
    public class Entry
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int DistanceInMeters { get; set; }
        public int TimeInSeconds { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
