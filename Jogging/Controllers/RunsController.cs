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
    [Route("api/[controller]"), Authorize]
    public class RunsController : Controller
    {
        JoggingContext _context;
        IUserService _userService;
        public RunsController(JoggingContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Get(DateTime from, DateTime to, int userId)
        {
            var currentUser = _context.Users.FindByEmail(_userService.GetCurrentUserIdentity());
            if (currentUser.Role != UserRole.Admin && userId != currentUser.Id)
            {
                ModelState.AddModelError(nameof(userId), "Access denied");
                return BadRequest(ModelState);
            }

            var entries = _context.Entries
                .Where(e => e.Date >= from && e.Date <= to)
                .Where(e => e.UserId == userId)
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

            return Ok(entries);
        }

        [HttpGet("[action]")]
        public IEnumerable<WeeklySummaryDTO> WeeklySummary(DateTime from, DateTime to)
        {
            return _context.Entries.Where(e => e.Date >= from && e.Date <= to)
                .Where(e => e.User.Email.ToUpper() == _userService.GetCurrentUserIdentity().ToUpper())
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
