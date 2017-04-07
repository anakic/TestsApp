using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using jogging.Model;
using Microsoft.EntityFrameworkCore;
using jogging.Services;
using Microsoft.AspNetCore.Authorization;

namespace jogging.Controllers
{
    [Route("api/[controller]")]
    public class RunsController : Controller
    {
        JoggingContext _context;
        
        public RunsController(JoggingContext context)
        {
            _context = context;
        }

        [HttpGet()]
        public IEnumerable<EntryDTO> Get(DateTime from, DateTime to, int? userId)
        {
            return _context.Entries.Where(e => e.Date >= from && e.Date <= to)
                .Select(e => new EntryDTO()
                {
                    Id = e.Id,
                    UserId = e.User.Id,
                    Date = e.Date,
                    DistanceInMeters = e.DistanceInMeters,
                    TimeInSeconds = e.TimeInSeconds,
                    UserEmail = e.User.Email,
                    AverageSpeed = e.DistanceInMeters / (float)e.TimeInSeconds
                });
        }

        [HttpGet("[action]")]
        public IEnumerable<WeeklySummaryDTO> WeeklySummary(DateTime from, DateTime to)
        {
            return _context.Entries.Where(e => e.Date >= from && e.Date <= to)
                .GroupBy(e => new { Year = e.Date.Year, Week = e.Date.Day / 7 })
                .Select(g => new WeeklySummaryDTO
                {
                    Year = g.Key.Year,
                    Week = g.Key.Week,
                    TotalDistanceInMeters = g.Sum(e => e.DistanceInMeters),
                    TotalTimeInSecoonds = g.Sum(e => e.TimeInSeconds),
                    AverageSpeed = g.Sum(e => e.DistanceInMeters) / (float)g.Sum(e => e.TimeInSeconds)
                });
        }
    }

    public class WeeklySummaryDTO
    {
        public int Year { get; set; }
        public int Week { get; set; }
        public int TotalDistanceInMeters { get; set; }
        public int TotalTimeInSecoonds { get; set; }
        public float AverageSpeed { get; set; }
    }

    public class EntryDTO
    {
        public float AverageSpeed { get; set; }
        public string UserEmail { get; set; }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int DistanceInMeters { get; set; }
        public int TimeInSeconds { get; set; }
        public int UserId { get; set; }

        public EntryDTO() { }
    }
}
