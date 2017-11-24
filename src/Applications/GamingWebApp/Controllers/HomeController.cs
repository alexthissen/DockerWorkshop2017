using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GamingWebApp.Models;
using GamingWebApp.Proxies;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace GamingWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;

        public HomeController(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<HomeController>();
            this.configuration = configuration;
        }
        public async Task<IActionResult> Index()
        {
            LeaderboardProxy proxy = new LeaderboardProxy(configuration["LeaderboardWebApiBaseUrl"], logger);
            var leaderboard = await proxy.GetLeaderboardAsync();
            return View(leaderboard);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
