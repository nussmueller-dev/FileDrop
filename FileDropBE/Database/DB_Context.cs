﻿using FileDropBE.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileDropBE.Database {
  public class DB_Context : DbContext {
    public DB_Context(DbContextOptions<DB_Context> options) : base(options) {
      Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<File> Files { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
      modelBuilder.Entity<User>().ToTable("User");
      modelBuilder.Entity<File>().ToTable("Files");
    }
  }
}
