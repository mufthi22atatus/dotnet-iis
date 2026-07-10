using System.Threading.Tasks;

namespace TaskManager.Services
{
    public class WeatherSnapshot
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double TemperatureC { get; set; }
        public double WindKph { get; set; }
        public string Source { get; set; }
    }

    public class GitHubRepoInfo
    {
        public string FullName { get; set; }
        public int Stargazers { get; set; }
        public int OpenIssues { get; set; }
        public string Description { get; set; }
    }

    public interface IExternalApiClient
    {
        Task<WeatherSnapshot> GetWeatherAsync(double lat, double lon);
        Task<GitHubRepoInfo> GetRepoAsync(string owner, string repo);
    }
}
