using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Leaderboard.WebAPI.Models
{
    public class Score
    {
        public int Id { get; set; }
        public int Points { get; set; }
        public string Game { get; set; }
        public int GamerId { get; set; }
        [ForeignKey("GamerId")]
        public Gamer Gamer { get; set; }
    }
}