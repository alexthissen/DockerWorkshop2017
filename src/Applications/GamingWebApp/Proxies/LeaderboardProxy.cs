using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GamingWebApp.Proxies
{
    public class LeaderboardProxy
    {
        private readonly string hostUri = "http://localhost:31741";
        private readonly string leaderboardApiEndpoint = "/api/leaderboard";
        private ILogger logger;

        public LeaderboardProxy(string hostUri, ILogger logger)
        {
            this.logger = logger;
            this.hostUri = hostUri;
        }

        public async Task<IEnumerable<dynamic>> GetLeaderboardAsync()
        {
            HttpResponseMessage response = null;
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(
                    HttpMethod.Get,
                    hostUri + leaderboardApiEndpoint);
                response = await client.SendAsync(request);
                if (!response.IsSuccessStatusCode) return new List<string>();

                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<dynamic>>(content);
            }
            catch (HttpRequestException)
            {
                return new List<string>();
            }
        }
    }
}
