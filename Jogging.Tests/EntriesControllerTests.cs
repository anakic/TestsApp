using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using jogging.Model;
using Microsoft.EntityFrameworkCore;
using jogging.Controllers;
using Jogging.Tests.Mocks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Jogging.Tests
{
    [TestClass]
    public class EntriesControllerTests
    {
        User _regularUser1;
        User _regularUser2;
        User _adminUser;
        EntriesController _controller;
        JoggingDbContext _context;
        DummyUserService _userService;

        [TestInitialize()]
        public void Initialize()
        {
            var optionsBuilder = new DbContextOptionsBuilder<JoggingDbContext>();
            optionsBuilder.UseInMemoryDatabase();
            _context = new JoggingDbContext(optionsBuilder.Options);

            _context.Users.RemoveRange(_context.Users);
            _context.Entries.RemoveRange(_context.Entries);
            _context.SaveChanges();

            _context.Users.Add(_regularUser1 = new User() { Id = 123, Email = "regular1@user1.com1", FirstName = "Joe", LastName = "User", Role = UserRole.User });
            _context.Users.Add(_regularUser2 = new User() { Id = 234, Email = "regular2@user2.com2", FirstName = "Moe", LastName = "User", Role = UserRole.User });
            _context.Users.Add(_adminUser = new User() { Id = 345, Email = "le@admin.com", FirstName = "Le", LastName = "Admin", Role = UserRole.Admin });
            _context.SaveChanges();

            _userService = new DummyUserService(_regularUser1.Email);
            _controller = new EntriesController(_context, _userService);
        }

        [TestMethod]
        public void GetsEntriesInRange()
        {
            DateTime dt1 = new DateTime(2020, 1, 15);
            DateTime dt2 = new DateTime(2020, 1, 16);
            DateTime dt3 = new DateTime(2020, 1, 17);
            DateTime dt4 = new DateTime(2020, 1, 18);
            DateTime dt5 = new DateTime(2020, 1, 19);

            _context.Add(new Entry() { Id = 1, Date = dt1, UserId = _regularUser1.Id });
            _context.Add(new Entry() { Id = 2, Date = dt2, UserId = _regularUser1.Id });
            _context.Add(new Entry() { Id = 3, Date = dt3, UserId = _regularUser1.Id });
            _context.Add(new Entry() { Id = 4, Date = dt4, UserId = _regularUser1.Id });
            _context.Add(new Entry() { Id = 5, Date = dt5, UserId = _regularUser1.Id });
            _context.SaveChanges();

            var res = ((_controller.Get(dt2, dt4, _regularUser1.Id) as OkObjectResult).Value as IEnumerable<EntryDTO>).ToArray();
            Assert.AreEqual(3, res.Count());
            Assert.AreEqual(1, res.Count(e => e.Id == 2));//include from
            Assert.AreEqual(1, res.Count(e => e.Id == 3));
            Assert.AreEqual(1, res.Count(e => e.Id == 4));//include to
        }

        [TestMethod]
        public void UserGetsOnlyOwnEntriesInPeriod()
        {
            DateTime dt1 = new DateTime(2020, 1, 16);
            DateTime dt2 = new DateTime(2020, 1, 16);
            
            //add entry for user1
            _context.Add(new Entry() { Id = 1, Date = dt1, UserId = _regularUser1.Id });
            //add two entries for user 2
            _context.Add(new Entry() { Id = 2, Date = dt1, UserId = _regularUser2.Id });
            _context.Add(new Entry() { Id = 3, Date = dt2, UserId = _regularUser2.Id });
            _context.SaveChanges();

            //verify only user1 entries retrieved when user1 active
            _userService.SetUser(_regularUser1.Email);
            var res = ((_controller.Get(dt1, dt2, _regularUser1.Id) as OkObjectResult).Value as IEnumerable<EntryDTO>).ToArray();
            Assert.AreEqual(1, res.Single().Id);

            //switch to user2 and verify only user2 entries returned
            _userService.SetUser(_regularUser2.Email);
            res = ((_controller.Get(dt1, dt2, _regularUser2.Id) as OkObjectResult).Value as IEnumerable<EntryDTO>).ToArray();
            Assert.AreEqual(1, res.Count(e => e.Id == 2));
            Assert.AreEqual(1, res.Count(e => e.Id == 3));
        }

        [TestMethod]
        public void UserCannotAccessAnotherUsersEntries()
        {
            DateTime dt1 = new DateTime(2020, 1, 16);

            _context.Add(new Entry() { Id = 1, Date = dt1, UserId = _regularUser2.Id });
            _context.SaveChanges();

            var res = _controller.Get(dt1, dt1, _regularUser2.Id);
            Assert.IsInstanceOfType(res, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public void AdminCanAccessAnotherUsersEntries()
        {
            DateTime dt1 = new DateTime(2020, 1, 16);
            DateTime dt2 = new DateTime(2020, 1, 17);

            _userService.SetUser(_adminUser.Email);
            _context.Add(new Entry() { Id = 1, Date = dt1, UserId = _regularUser1.Id });
            _context.Add(new Entry() { Id = 2, Date = dt1, UserId = _regularUser2.Id });
            _context.Add(new Entry() { Id = 3, Date = dt2, UserId = _regularUser2.Id });
            _context.SaveChanges();

            //user 1 entries
            var res = ((_controller.Get(dt1, dt2, _regularUser1.Id) as OkObjectResult).Value as IEnumerable<EntryDTO>).ToArray();
            Assert.AreEqual(1, res.Single().Id);

            //user 2 entries
            res = ((_controller.Get(dt1, dt2, _regularUser2.Id) as OkObjectResult).Value as IEnumerable<EntryDTO>).ToArray();
            Assert.AreEqual(1, res.Count(e => e.Id == 2));
            Assert.AreEqual(1, res.Count(e => e.Id == 3));
        }

        [TestMethod]
        public void ProperlyMapsEntryPropertiesToDTO()
        {
            var dt1 = new DateTime(2020, 1, 16);
            var entry = new Entry() { Id = 1, Date = dt1, UserId = _regularUser1.Id, User = _regularUser1, TimeInSeconds = 12345, DistanceInMeters = 54321 };

            _context.Add(entry);
            _context.SaveChanges();

            var res = ((_controller.Get(dt1, dt1, _regularUser1.Id) as OkObjectResult).Value as IEnumerable<EntryDTO>).Single();
            Assert.AreEqual(res.Id, entry.Id);
            Assert.AreEqual(res.Date, entry.Date);
            Assert.AreEqual(res.TimeInSeconds, entry.TimeInSeconds);
            Assert.AreEqual(res.DistanceInMeters, entry.DistanceInMeters);
            Assert.AreEqual(res.AverageSpeed, entry.DistanceInMeters / (float)entry.TimeInSeconds);//in reality would probably be more appropriate to make it a precaltulated constant (instead of duplicating the calculation)
            Assert.AreEqual(res.UserEmail, _regularUser1.Email);
            Assert.AreEqual(res.UserId, _regularUser1.Id);
        }
    }
}
