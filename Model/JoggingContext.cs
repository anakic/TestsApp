﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace jogging.Model
{
    public class JoggingContext : DbContext
    {
        public JoggingContext(DbContextOptions options) : base(options){}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(u => u.Email);
            modelBuilder.Entity<User>().HasMany(u=>u.Entries).WithOne(e=>e.User);

            modelBuilder.Entity<Entry>().HasOne(e => e.User).WithMany(u=>u.Entries);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Entry> Entries { get; set; }
        public DbSet<User> Users { get; set; }
    }

    public static class ContextExtensionMethods
    {
        public static User FindByEmail(this DbSet<User> users, string email)
        {
            return users.SingleOrDefault(u => u.Email.ToUpper() == email.ToUpper());
        }
    }
}
