using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tally.Models;

namespace Tally.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Course> Course { get; set; }
        public DbSet<CourseUser> CourseUser { get; set; }
        public DbSet<Lecture> Lecture { get; set; }
        public DbSet<Signature> Signature { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CourseUser>()
           .HasKey(t => new { t.CourseId, t.Id });

            builder.Entity<CourseUser>()
                .HasOne(pt => pt.Course)
                .WithMany(p => p.CourseUsers)
                .HasForeignKey(pt => pt.CourseId);

            builder.Entity<CourseUser>()
                .HasOne(pt => pt.User)
                .WithMany(t => t.CourseUsers)
                .HasForeignKey(pt => pt.Id);

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

    }
}
