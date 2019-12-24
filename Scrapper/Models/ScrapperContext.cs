using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scrapper.Models
{
    public class ScrapperContext : DbContext
    {
        public ScrapperContext(DbContextOptions<ScrapperContext> options) : base(options)
        {
        }
        public DbSet<User> users { get; set; }
        public DbSet<Backlog> backlogs { get; set; }
        public DbSet<UserTweets> usertweets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Define composite key.
            //builder.Entity<User_Tweets>()
            //    .HasKey(lc => new { lc.UserId});

        }

    }
}
