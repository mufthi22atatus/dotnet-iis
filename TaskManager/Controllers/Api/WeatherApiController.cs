using System.Threading.Tasks;
using System.Web.Http;
using TaskManager.Services;

namespace TaskManager.Controllers.Api
{
    [Authorize]
    [RoutePrefix("api/external")]
    public class WeatherApiController : ApiController
    {
        private IExternalApiClient Ext => DependencyConfig.Resolve<IExternalApiClient>();

        [HttpGet, Route("weather")]
        public async Task<IHttpActionResult> Weather(double lat = 13.0827, double lon = 80.2707)
        {
            var snap = await Ext.GetWeatherAsync(lat, lon);
            return Ok(snap);
        }

        [HttpGet, Route("github/{owner}/{repo}")]
        public async Task<IHttpActionResult> GitHub(string owner, string repo)
        {
            var info = await Ext.GetRepoAsync(owner, repo);
            return info == null ? (IHttpActionResult)NotFound() : Ok(info);
        }
    }
}
