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
    public class EntriesController : Controller
    {
        JoggingDbContext _context;
        ILoginService _userService;
        public EntriesController(JoggingDbContext context, ILoginService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpPut("{entryId}")]
        public IActionResult Update(int entryId, [FromBody]EntryDataDTO updatedEntryDTO)
        {
            var entry = _context.Entries.Find(entryId);
            var currentUser = _userService.GetCurrentUser();
            if (entry == null)
            {
                return BadRequest("Invalid entry ID.");
            }//if the current user is not the admin, we require both the current and new userId to be the same as current userID in order to allow the change
            else if (currentUser.Role != UserRole.Admin && (currentUser.Id != updatedEntryDTO.UserId || entry.UserId != currentUser.Id))
            {
                return BadRequest("Only admin users may change the user an entry belongs to.");
            }
            else
            {
                entry.Date = updatedEntryDTO.Date;
                entry.DistanceInMeters = updatedEntryDTO.DistanceInMeters;
                entry.TimeInSeconds = updatedEntryDTO.TimeInSeconds;
                entry.UserId = updatedEntryDTO.UserId;
                _context.SaveChanges();
                return Ok();
            }
        }

        [HttpPost()]
        public IActionResult Create([FromBody]EntryDataDTO newEntryDTO)
        {
            if (_context.Users.Find(newEntryDTO.UserId) == null)
            {
                return BadRequest("Invalid userId.");
            }
            if (_userService.GetCurrentUser().CanAccessEntriesForUser(newEntryDTO.UserId) == false)
            {
                return BadRequest("Access denied.");
            }
            else
            {
                var entry = new Entry()
                {
                    Date = newEntryDTO.Date,
                    DistanceInMeters = newEntryDTO.DistanceInMeters,
                    TimeInSeconds = newEntryDTO.TimeInSeconds,
                    UserId = newEntryDTO.UserId
                };
                _context.Entries.Add(entry);
                _context.SaveChanges();
                return Ok();
            }
        }

        [HttpGet]
        public IActionResult Get(DateTime from, DateTime to, int? userId)
        {
            var currentUser = _userService.GetCurrentUser();
            userId = userId ?? currentUser.Id;//assume current user if not specified

            if (currentUser.CanAccessEntriesForUser(userId.Value) == false)
            {
                return BadRequest("Access denied");
            }
            else
            {
                var entries = _context.Entries
                    .Where(e => e.Date >= from && e.Date <= to)
                    .Where(e => e.UserId == userId.Value)
                    .OrderByDescending(e => e.Date)
                    .Select(e => new EntryDTO()
                    {
                        Id = e.Id,
                        UserId = e.User.Id,
                        Date = e.Date,
                        DistanceInMeters = e.DistanceInMeters,
                        TimeInSeconds = e.TimeInSeconds,
                        UserEmail = e.User.Email,
                    });
                return Ok(entries);
            }
        }

        [HttpDelete("{id}")]
        public StatusCodeResult Delete(int id)
        {
            _context.Entries.Remove(_context.Entries.Find(id));
            _context.SaveChanges();
            return Ok();
        }

        [HttpGet("[action]")]
        public IActionResult WeeklySummaries(DateTime from, DateTime to)
        {
            var summaries = _context.Entries.Where(e => e.Date >= from && e.Date <= to)
                .Where(e => e.User.Email.ToUpper() == _userService.GetCurrentUser().Email.ToUpper())
                .GroupBy(e => new { Year = e.Date.Year, Week = CalculateWeekOfYear(e.Date) })
                .Select(g => new WeeklySummaryDTO
                {
                    Year = g.Key.Year,
                    Week = g.Key.Week,
                    TotalDistanceInMeters = g.Sum(e => e.DistanceInMeters),
                    TotalTimeInSeconds = g.Sum(e => e.TimeInSeconds)
                })
                .OrderByDescending(e => e.Year)
                .ThenByDescending(e => e.Week);

            return Ok(summaries);
        }

        private static int CalculateWeekOfYear(DateTime dt)
        {
            //.NET core doesn't have a Calendar.WeekOfYear() method
            //so using an (not so great) approximation here
            return 1 + dt.DayOfYear / 7;
        }
    }

    #region viewmodels
    public class WeeklySummaryDTO
    {
        public int Year { get; set; }
        public int Week { get; set; }
        public int TotalDistanceInMeters { get; set; }
        public int TotalTimeInSeconds { get; set; }
        public float AverageSpeed { get { return TotalDistanceInMeters / (float)TotalTimeInSeconds; } }
    }

    public class EntryDataDTO
    {
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public int DistanceInMeters { get; set; }
        public int TimeInSeconds { get; set; }
    }

    public class EntryDTO
    {
        public string UserEmail { get; set; }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int DistanceInMeters { get; set; }
        public int TimeInSeconds { get; set; }
        public int UserId { get; set; }

        public float AverageSpeed { get { return DistanceInMeters / (float)TimeInSeconds; } }

        public EntryDTO() { }
    }
    #endregion
}
