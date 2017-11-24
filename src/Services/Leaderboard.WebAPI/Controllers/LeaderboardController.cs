using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Leaderboard.WebAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Leaderboard.WebAPI.Controllers
{
    public class HighScore
    {
        public string Game { get; set; }
        public string Nickname { get; set; }
        public int Points { get; set; }
    }

    [Route("api/[controller]")]
    public class LeaderboardController : Controller
    {
        private readonly LeaderboardContext context;

        public LeaderboardController(LeaderboardContext context)
        {
            this.context = context;
        }

        // GET api/leaderboard
        [HttpGet]
        public async Task<IEnumerable<HighScore>> Get()
        {
            var scores = from score in context.Scores
                         group new { score.Gamer.Nickname, score.Points } by score.Game into scoresPerGame
                         select new HighScore()
                         {
                             Game = scoresPerGame.Key,
                             Points = scoresPerGame.Max(e => e.Points),
                             Nickname = scoresPerGame.OrderByDescending(s => s.Points).First().Nickname
                         };
            return await scores.ToListAsync();
        }
    }
}
