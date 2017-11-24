using Leaderboard.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leaderboard.WebAPI.Infrastructure
{
    public class LeaderboardContext: DbContext
    {
        public LeaderboardContext(DbContextOptions<LeaderboardContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gamer>().ToTable("Gamers");
            modelBuilder.Entity<Score>().ToTable("Scores");
        }

        public DbSet<Gamer> Gamers { get; set; }
        public DbSet<Score> Scores { get; set; }
    }
}
