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
            _context = new JoggingDbContext(new DbContextOptionsBuilder<JoggingDbContext>().UseInMemoryDatabase().Options);
            _context.Database.EnsureDeleted();//make sure we don't share state between tests

            _context.Users.Add(_regularUser1 = new User() { Id = 123, Email = "regular1@user1.com1", FirstName = "Joe", LastName = "User", Role = UserRole.User });
            _context.Users.Add(_regularUser2 = new User() { Id = 234, Email = "regular2@user2.com2", FirstName = "Moe", LastName = "User", Role = UserRole.User });
            _context.Users.Add(_adminUser = new User() { Id = 345, Email = "le@admin.com", FirstName = "Le", LastName = "Admin", Role = UserRole.Admin });
            _context.SaveChanges();

            _userService = new DummyUserService(_regularUser1);
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
            _userService.SetUser(_regularUser1);
            var res = ((_controller.Get(dt1, dt2, _regularUser1.Id) as OkObjectResult).Value as IEnumerable<EntryDTO>).ToArray();
            Assert.AreEqual(1, res.Single().Id);

            //switch to user2 and verify only user2 entries returned
            _userService.SetUser(_regularUser2);
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
        public void DeleteEntryWorks()
        {
            DateTime dt1 = new DateTime(2020, 1, 16);

            _userService.SetUser(_regularUser1);
            _context.Add(new Entry() { Id = 1, Date = dt1, UserId = _regularUser1.Id });
            _context.Add(new Entry() { Id = 2, Date = dt1, UserId = _regularUser1.Id });
            _context.SaveChanges();

            _controller.Delete(1);
            var res = ((_controller.Get(dt1, dt1, null) as OkObjectResult).Value as IEnumerable<EntryDTO>).ToArray();
            Assert.AreEqual(2, res.Single().Id);
        }

        [TestMethod]
        public void NullUserAssumesCurrentUser()
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
            _userService.SetUser(_regularUser1);
            var res = ((_controller.Get(dt1, dt2, null) as OkObjectResult).Value as IEnumerable<EntryDTO>).ToArray();
            Assert.AreEqual(1, res.Single().Id);

            //switch to user2 and verify only user2 entries returned
            _userService.SetUser(_regularUser2);
            res = ((_controller.Get(dt1, dt2, null) as OkObjectResult).Value as IEnumerable<EntryDTO>).ToArray();
            Assert.AreEqual(1, res.Count(e => e.Id == 2));
            Assert.AreEqual(1, res.Count(e => e.Id == 3));
        }

        [TestMethod]
        public void AdminCanAccessAnotherUsersEntries()
        {
            DateTime dt1 = new DateTime(2020, 1, 16);
            DateTime dt2 = new DateTime(2020, 1, 17);

            _userService.SetUser(_adminUser);
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

        [TestMethod]
        public void WeeklySummaryIsCorrect()
        {
            DateTime dt1 = new DateTime(2020, 1, 12);//week2
            DateTime dt2 = new DateTime(2020, 1, 18);//week3
            DateTime dt3 = new DateTime(2020, 1, 19);//week3

            Entry u1e1, u2e1, u2e2, u2e3;//u2e2 means user2:entry2
            _userService.SetUser(_adminUser);
            _context.Add(u1e1 = new Entry() { Id = 1, DistanceInMeters = 999, TimeInSeconds = 600, Date = dt1, UserId = _regularUser1.Id, User = _regularUser1 });
            _context.Add(u2e1 = new Entry() { Id = 2, DistanceInMeters = 1999, TimeInSeconds = 1000, Date = dt1, UserId = _regularUser2.Id, User = _regularUser2 });
            _context.Add(u2e2 = new Entry() { Id = 3, DistanceInMeters = 2999, TimeInSeconds = 1500, Date = dt2, UserId = _regularUser2.Id, User = _regularUser2 });
            _context.Add(u2e3 = new Entry() { Id = 4, DistanceInMeters = 3999, TimeInSeconds = 1900, Date = dt3, UserId = _regularUser2.Id, User = _regularUser2 });
            _context.SaveChanges();

            //user 1 summary (only week2)
            _userService.SetUser(_regularUser1);
            var res = ((_controller.WeeklySummary(dt1, dt3) as OkObjectResult).Value as IEnumerable<WeeklySummaryDTO>).ToArray();
            Assert.AreEqual(2020, res.Single().Year);
            Assert.AreEqual(2, res.Single().Week);
            Assert.AreEqual(u1e1.DistanceInMeters, res.Single().TotalDistanceInMeters);
            Assert.AreEqual(u1e1.TimeInSeconds, res.Single().TotalTimeInSecoonds);
            Assert.AreEqual(u1e1.DistanceInMeters / (float)u1e1.TimeInSeconds, res.Single().AverageSpeed);

            //user 2
            _userService.SetUser(_regularUser2);
            res = ((_controller.WeeklySummary(dt1, dt3) as OkObjectResult).Value as IEnumerable<WeeklySummaryDTO>).ToArray();
            Assert.AreEqual(2, res.Count());
            //--> summary for week 3
            var u2w3 = res.ElementAt(0);
            Assert.AreEqual(2020, u2w3.Year);
            Assert.AreEqual(3, u2w3.Week);
            Assert.AreEqual(u2e2.DistanceInMeters + u2e3.DistanceInMeters, u2w3.TotalDistanceInMeters);
            Assert.AreEqual(u2e2.TimeInSeconds + u2e3.TimeInSeconds, u2w3.TotalTimeInSecoonds);
            Assert.AreEqual((u2e2.DistanceInMeters + u2e3.DistanceInMeters) / (float)(u2e2.TimeInSeconds + u2e3.TimeInSeconds), u2w3.AverageSpeed);
            //--> summary for week 2
            var u2w2 = res.ElementAt(1);
            Assert.AreEqual(2020, u2w2.Year);
            Assert.AreEqual(2, u2w2.Week);
            Assert.AreEqual(u2e1.DistanceInMeters, u2w2.TotalDistanceInMeters);
            Assert.AreEqual(u2e1.TimeInSeconds, u2w2.TotalTimeInSecoonds);
            Assert.AreEqual(u2e1.DistanceInMeters / (float)u2e1.TimeInSeconds, u2w2.AverageSpeed);
        }


        [TestMethod]
        public void UpdatesWithValidData()
        {
            DateTime dtInitial = new DateTime(2020, 1, 1);
            DateTime dtChanged = new DateTime(2020, 2, 2);

            Entry entry;
            _userService.SetUser(_regularUser1);
            _context.Add(entry = new Entry() { Id = 123, Date = dtInitial, UserId = _regularUser1.Id, DistanceInMeters = 55, TimeInSeconds = 99 });
            _context.SaveChanges();

            _controller.Update(new EntryUpdateDTO()
            {
                Id = entry.Id,
                DistanceInMeters = entry.DistanceInMeters + 101,
                TimeInSeconds = entry.TimeInSeconds + 102,
                UserId = entry.UserId,
                Date = dtChanged
            });

            var res = ((_controller.Get(dtChanged, dtChanged, _regularUser1.Id) as OkObjectResult).Value as IEnumerable<EntryDTO>).ToArray();
            Assert.AreEqual(123, res.Single().Id);
            Assert.AreEqual(156, res.Single().DistanceInMeters);
            Assert.AreEqual(201, res.Single().TimeInSeconds);
            Assert.AreEqual(dtChanged, res.Single().Date);
        }
        
        [TestMethod]
        public void UpdateWithIvalidIdFails()
        {
            DateTime dtInitial = new DateTime(2020, 1, 1);
            DateTime dtChanged = new DateTime(2020, 2, 2);

            Entry entry;
            _context.Add(entry = new Entry() { Id = 123, Date = dtInitial, UserId = _regularUser1.Id, DistanceInMeters = 55, TimeInSeconds = 99 });
            _context.SaveChanges();

            _userService.SetUser(_regularUser1);
            var res = _controller.Update(new EntryUpdateDTO()
            {
                Id = entry.Id + 1,//invalid id
                DistanceInMeters = entry.DistanceInMeters + 101,
                TimeInSeconds = entry.TimeInSeconds + 102,
                UserId = entry.UserId,
                Date = dtChanged
            });

            Assert.AreEqual((res as BadRequestObjectResult).Value, "Invalid entry ID.");
        }

        [TestMethod]
        public void MoveEntryToOtherUserFailsIfNotAdmin()
        {
            DateTime dtInitial = new DateTime(2020, 1, 1);
            DateTime dtChanged = new DateTime(2020, 2, 2);

            Entry entry;
            _context.Add(entry = new Entry() { Id = 123, Date = dtInitial, UserId = _regularUser1.Id, DistanceInMeters = 55, TimeInSeconds = 99 });
            _context.SaveChanges();

            _userService.SetUser(_regularUser1);
            var res = _controller.Update(new EntryUpdateDTO()
            {
                Id = entry.Id,
                DistanceInMeters = entry.DistanceInMeters + 101,
                TimeInSeconds = entry.TimeInSeconds + 102,
                UserId = _regularUser2.Id,
                Date = dtChanged
            });

            Assert.AreEqual((res as BadRequestObjectResult).Value, "Only admin users may change the user an entry belongs to.");
        }

        [TestMethod]
        public void MoveEntryFromOtherUserFailsIfNotAdmin()
        {
            DateTime dtInitial = new DateTime(2020, 1, 1);
            DateTime dtChanged = new DateTime(2020, 2, 2);

            Entry entry;
            _context.Add(entry = new Entry() { Id = 123, Date = dtInitial, UserId = _regularUser1.Id, DistanceInMeters = 55, TimeInSeconds = 99 });
            _context.SaveChanges();

            _userService.SetUser(_regularUser2);
            var res = _controller.Update(new EntryUpdateDTO()
            {
                Id = entry.Id,
                DistanceInMeters = entry.DistanceInMeters + 101,
                TimeInSeconds = entry.TimeInSeconds + 102,
                UserId = _regularUser2.Id,
                Date = dtChanged
            });

            Assert.AreEqual((res as BadRequestObjectResult).Value, "Only admin users may change the user an entry belongs to.");
        }

        [TestMethod]
        public void ChangeUserOnEntrySucceedsIfAdmin()
        {
            DateTime dtInitial = new DateTime(2020, 1, 1);
            DateTime dtChanged = new DateTime(2020, 2, 2);

            Entry entry;
            _context.Add(entry = new Entry() { Id = 123, Date = dtInitial, UserId = _regularUser1.Id, DistanceInMeters = 55, TimeInSeconds = 99 });
            _context.SaveChanges();

            _userService.SetUser(_adminUser);
            EntryUpdateDTO updateDTO;
            var res = _controller.Update(updateDTO = new EntryUpdateDTO()
            {
                Id = entry.Id,
                DistanceInMeters = entry.DistanceInMeters + 101,
                TimeInSeconds = entry.TimeInSeconds + 102,
                UserId = _regularUser2.Id,
                Date = dtChanged
            });

            Assert.IsInstanceOfType(res, typeof(OkResult));

            var res2 = ((_controller.Get(dtChanged, dtChanged, _regularUser2.Id) as OkObjectResult).Value as IEnumerable<EntryDTO>).ToArray();
            Assert.AreEqual(updateDTO.Id, res2.Single().Id);
            Assert.AreEqual(updateDTO.DistanceInMeters, res2.Single().DistanceInMeters);
            Assert.AreEqual(updateDTO.TimeInSeconds, res2.Single().TimeInSeconds);
            Assert.AreEqual(updateDTO.Date, res2.Single().Date);
        }

        [TestMethod]
        public void NewEntryWithValidData()
        {
            EntryNewDTO entryNew;
            _userService.SetUser(_regularUser1);
            _controller.Create(entryNew = new EntryNewDTO()
            {
                DistanceInMeters = 101,
                TimeInSeconds = 102,
                UserId = _regularUser1.Id,
                Date = new DateTime(2020, 1, 1)
            });

            var res = ((_controller.Get(entryNew.Date, entryNew.Date, _regularUser1.Id) as OkObjectResult).Value as IEnumerable<EntryDTO>).ToArray();
            Assert.AreEqual(entryNew.DistanceInMeters, res.Single().DistanceInMeters);
            Assert.AreEqual(entryNew.TimeInSeconds, res.Single().TimeInSeconds);
            Assert.AreEqual(entryNew.Date, res.Single().Date);
        }

        [TestMethod]
        public void NewEntryWithBadUserIdData()
        {
            EntryNewDTO entryNew;
            _userService.SetUser(_regularUser1);
            var res = _controller.Create(entryNew = new EntryNewDTO()
            {
                DistanceInMeters = 101,
                TimeInSeconds = 102,
                UserId = 999999,//bad id
                Date = new DateTime(2020, 1, 1)
            });

            Assert.AreEqual("Invalid userId.", (res as BadRequestObjectResult).Value);
        }

        [TestMethod]
        public void UserCantCreateEntryForAnotherUser()
        {
            _userService.SetUser(_regularUser1);
            EntryNewDTO entryNew;
            var res = _controller.Create(entryNew = new EntryNewDTO()
            {
                DistanceInMeters = 101,
                TimeInSeconds = 102,
                UserId = _regularUser2.Id,//other user
                Date = new DateTime(2020, 1, 1)
            });

            Assert.AreEqual("Access denied.", (res as BadRequestObjectResult).Value);
        }

        [TestMethod]
        public void AdminCanCreateEntryForAnotherUser()
        {
            _userService.SetUser(_adminUser);
            EntryNewDTO entryNew;
            var res = _controller.Create(entryNew = new EntryNewDTO()
            {
                DistanceInMeters = 101,
                TimeInSeconds = 102,
                UserId = _regularUser2.Id,//other user
                Date = new DateTime(2020, 1, 1)
            });

            Assert.IsInstanceOfType(res, typeof(OkResult));
        }
    }
}
