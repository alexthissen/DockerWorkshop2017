using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Leaderboard.WebAPI.Models;

namespace Leaderboard.WebAPI.Infrastructure
{
    public class DbInitializer
    {
        public async static Task Initialize(LeaderboardContext context)
        {
            context.Database.EnsureCreated();
            if (context.Gamers.Any())
            {
                return;
            }
            context.Gamers.Add(new Gamer() { GamerGuid = Guid.NewGuid(), Nickname = "LX360" });
            context.Gamers.Add(new Gamer() { GamerGuid = Guid.NewGuid(), Nickname = "Kn3luZ" });
            context.Gamers.Add(new Gamer() { GamerGuid = Guid.NewGuid(), Nickname = "BeyondDotNET" });
            await context.SaveChangesAsync();
        }
    }
}
