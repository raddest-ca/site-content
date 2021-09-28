using System;
using System.Collections.Generic;
using app.Models;
using Microsoft.EntityFrameworkCore;

namespace app.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public DbSet<Post> Posts {get; set;}
        public DbSet<File> Files {get; set;}
    }
}