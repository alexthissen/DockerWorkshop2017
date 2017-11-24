using GamingWebApp.Models;
using GamingWebApp.Proxies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GamingWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger logger;
        private readonly IOptionsSnapshot<WebAppSettings> settings;

        public HomeController(IOptionsSnapshot<WebAppSettings> settings, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<HomeController>();
            this.settings = settings;
        }
        public async Task<IActionResult> Index()
        {
            LeaderboardProxy proxy = new LeaderboardProxy(settings.Value.LeaderboardWebApiBaseUrl, logger);
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
