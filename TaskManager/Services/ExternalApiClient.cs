using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace TaskManager.Services
{
    public class ExternalApiClient : IExternalApiClient, IDisposable
    {
        // One HttpClient per AppDomain — reused across requests so APMs see a stable
        // outbound client and we don't blow through TCP sockets.
        private static readonly HttpClient Http = CreateClient();

        private static HttpClient CreateClient()
        {
            var c = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(15)
            };
            c.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("TaskManager", "1.0"));
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return c;
        }

        public async Task<WeatherSnapshot> GetWeatherAsync(double lat, double lon)
        {
            var baseUrl = ConfigurationManager.AppSettings["External:WeatherApiUrl"]
                          ?? "https://api.open-meteo.com/v1/forecast";
            var url = $"{baseUrl}?latitude={lat}&longitude={lon}&current_weather=true";

            AppLogger.Create<ExternalApiClient>()?.LogInformation("External GET {Url}", url);
            using (var resp = await Http.GetAsync(url))
            {
                resp.EnsureSuccessStatusCode();
                var json = await resp.Content.ReadAsStringAsync();
                var doc = JObject.Parse(json);
                var current = doc["current_weather"];
                return new WeatherSnapshot
                {
                    Latitude = lat,
                    Longitude = lon,
                    TemperatureC = current?.Value<double>("temperature") ?? 0,
                    WindKph = current?.Value<double>("windspeed") ?? 0,
                    Source = "open-meteo"
                };
            }
        }

        public async Task<GitHubRepoInfo> GetRepoAsync(string owner, string repo)
        {
            var baseUrl = ConfigurationManager.AppSettings["External:GitHubApiUrl"] ?? "https://api.github.com";
            var url = $"{baseUrl}/repos/{Uri.EscapeDataString(owner)}/{Uri.EscapeDataString(repo)}";

            using (var req = new HttpRequestMessage(HttpMethod.Get, url))
            {
                req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
                AppLogger.Create<ExternalApiClient>()?.LogInformation("External GET {Url}", url);
                using (var resp = await Http.SendAsync(req))
                {
                    if ((int)resp.StatusCode == 404) return null;
                    resp.EnsureSuccessStatusCode();
                    var json = await resp.Content.ReadAsStringAsync();
                    var doc = JObject.Parse(json);
                    return new GitHubRepoInfo
                    {
                        FullName = doc.Value<string>("full_name"),
                        Stargazers = doc.Value<int?>("stargazers_count") ?? 0,
                        OpenIssues = doc.Value<int?>("open_issues_count") ?? 0,
                        Description = doc.Value<string>("description")
                    };
                }
            }
        }

        public void Dispose() { /* HttpClient is static; do not dispose per-instance */ }
    }
}
